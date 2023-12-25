using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public enum MonsterEnum
{
    Slime,
    Spider,
    Nependis,
    HornetTurtle,
    Bat,
    RedSkeleton,
    GreenSkeleton,
    Goblin,
    DemonWizard
}

public class Monster : MonoBehaviourPun, IPunObservable
{
    private GameObject target = null;
    private DataRow[] dataRow;
    private NavMeshAgent nav;
    private Animator anim;
    [SerializeField]
    private Image hpBar;
    private MonsterSpawner spawner;
    [SerializeField] private GameObject _damageText;
    [SerializeField] private AudioSource onDamageSource;

    // 스탯
    [SerializeField]
    private MonsterEnum monsterEnum;
    public string monsterName;
    public int maxHp;
    public int currHp;
    public float damage;
    public int exp;
    public int money;
    public int recovery_hp;
    public float atkDelay = 1f;

    // 상태
    private bool isChase = false;
    private bool isDead = false;
    private bool isAttack = false;
    private bool isDamage = false;
    private bool atkReady = true;
    private Vector3 remotePos;
    private Quaternion remoteRot;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        GetMonsterDB();

        if (!PhotonNetwork.IsMasterClient) return;

        nav.updateRotation = false;

        InvokeRepeating("UpdateTarget", 0f, 0.25f);
    }

    // 스포너 설정(MonsterSpawner.cs 에서 호출)
    protected virtual void SetSpawner(string spawnerName)
    {
        transform.SetParent(GameObject.Find(spawnerName).transform);
        spawner = GetComponentInParent<MonsterSpawner>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (nav.enabled)
            {
                nav.isStopped = !isChase;

                if (target != null)
                    nav.SetDestination(target.transform.position);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
        }
    }


    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Rotation();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 호스트
        if (PhotonNetwork.IsMasterClient)
        {
            // 몬스터 위치값과 회전값을 브로드캐스트
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        // 호스트 외 클라이언트
        else
        {
            // 호스트로부터 전달받은 위치값과 회전값을 적용
            remotePos = (Vector3)stream.ReceiveNext();
            remoteRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // 타겟 설정
    private void UpdateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 10f, 1 << LayerMask.NameToLayer("Player"));

        if (cols.Length > 0)
        {

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].tag == "Player")
                {
                    target = cols[i].gameObject;
                    Chasing(true);
                }
            }
        }
        else
        {
            target = null;
            Chasing(false);
        }
    }

    private void GetMonsterDB()
    {
        dataRow = SystemManager.instance.dbManager.monsterTable.Select("name = '" + monsterEnum.ToString() + "'");

        monsterName = (string)dataRow[0]["name"];
        maxHp = (int)dataRow[0]["maxHp"];
        currHp = maxHp;
        damage = (float)dataRow[0]["damage"];
        exp = (int)dataRow[0]["experience"];
        money = (int)dataRow[0]["money"];
        recovery_hp = (int)dataRow[0]["recovery_hp"];
    }

    private void Rotation()
    {
        if (isChase && !isDead && !isAttack && !isDamage && target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
    }

    public void Chasing(bool onTrigger)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (onTrigger && !isAttack)
        {
            isChase = true;
            anim.SetBool("isWalk", true);
        }
        else
        {
            isChase = false;
            anim.SetBool("isWalk", false);
        }
    }

    public void StartAttack()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (!isDead && !isAttack && !isDamage && atkReady)
        {
            atkReady = false;
            isChase = false;
            isAttack = true;
            photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doAttack");
        }
    }

    private IEnumerator EndAttackAnim()
    {
        isChase = true;
        isAttack = false;
        yield return new WaitForSeconds(atkDelay);
        atkReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.tag == "PlayerAttack")
        {
            if (!isDead)
            {
                OnDamage(other.gameObject.GetComponentInParent<PlayerStat>().realDmg);
            }
        }
        else if (other.tag == "SkillAttack")
        {
            if (!isDead)
            {
                OnDamage(other.GetComponentInParent<Skill_1>().damage);
            }
        }
    }

    private void OnDamage(float damage)
    {
        isDamage = true;
        isAttack = false;
        isChase = false;

        float modifiedDamage = Random.Range(damage * 0.85f, damage * 1.15f);
        modifiedDamage = Mathf.RoundToInt(modifiedDamage);

        // 데미지 표시
        photonView.RPC("SpawnDamageText", RpcTarget.AllBuffered, transform.position, modifiedDamage);
        photonView.RPC("ApplyHp", RpcTarget.AllBuffered, modifiedDamage);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doDamage");

        if (currHp <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    // 데미지 표시 
    protected virtual void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject damageText = Instantiate(_damageText);
        damageText.transform.position = position;
        damageText.GetComponent<DamageText>().damage = damage;
    }

    protected virtual void ApplyHp(float damage)
    {
        currHp -= (int)damage;

        hpBar.fillAmount = (float)currHp / maxHp;

        onDamageSource.Play();
    }

    protected virtual void AnimTriggerRpc(string animStr)
    {
        if (anim != null)
            anim.SetTrigger(animStr);
    }

    // 피격 애니메이션 종료 시
    private void EndDamageAnim()
    {
        isChase = true;
        isDamage = false;
        atkReady = true;
    }

    private void OnDeath()
    {
        StopAllCoroutines();
        
        photonView.RPC("DeathRPC", RpcTarget.AllBuffered);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doDie");
        photonView.RPC("RewardRPC", RpcTarget.AllBuffered); // 보상

        StartCoroutine(DestroyAfter(gameObject, 4f));
        spawner.Despawn(gameObject);
    }

    // 포톤의 Network.Destroy()는 지연 파괴를 지원하지 않으므로 지연 파괴를 직접 구현함
    private IEnumerator DestroyAfter(GameObject target, float delay)
    {
        // delay 만큼 쉬고
        yield return new WaitForSeconds(delay);

        // target이 아직 파괴되지 않았다면
        if (target != null)
        {
            // target을 모든 네트워크 상에서 파괴
            PhotonNetwork.Destroy(target);
        }
    }

    protected virtual void DeathRPC()
    {
        isDead = true;
        isChase = false;
        nav.enabled = false;
        gameObject.layer = 7;  // MonsterDead Layer
    }

    protected virtual void RewardRPC()
    {
        // 보상
        IngameManager.instance.stat.SetExp(exp);
        IngameManager.instance.stat.money += money;
        IngameManager.instance.stat.currHp = Mathf.Clamp(IngameManager.instance.stat.currHp + recovery_hp, recovery_hp, IngameManager.instance.stat.maxHp);
    }
}
