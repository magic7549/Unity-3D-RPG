using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum BossEnum
{
    Golem,
    FlameDragon,
    ExperimentalX7Cain
}

public abstract class Boss : MonoBehaviourPun, IPunObservable
{
    private DataRow[] dataRow;
    protected Animator anim;
    protected Rigidbody rigid;
    private SkinnedMeshRenderer mesh;
    protected GameObject target;
    [SerializeField] private Material[] mat = new Material[2];
    [SerializeField] protected Image hpBar;
    [SerializeField] private GameObject damageText;
    [SerializeField] private AudioSource onDamageSource;

    // 스탯
    [SerializeField] private BossEnum bossEnum;
    public string monsterName;
    public int maxHp;
    public int currHp;
    public float damage;
    public float realDmg;
    public int exp;
    public int money;

    protected bool isRush = false;
    protected bool isLookTarget = true;
    protected bool isDie = false;
    private Vector3 remotePos;
    private Quaternion remoteRot;

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();

        GetMonsterDB();

        if (!PhotonNetwork.IsMasterClient) return;

        InvokeRepeating("UpdateTarget", 0f, 0.25f);
        StartCoroutine(Think());
    }

    private void GetMonsterDB()
    {
        dataRow = SystemManager.instance.dbManager.monsterTable.Select("name = '" + bossEnum.ToString() + "'");

        monsterName = (string)dataRow[0]["name"];
        maxHp = (int)dataRow[0]["maxHp"];
        currHp = maxHp;
        damage = (float)dataRow[0]["damage"];
        exp = (int)dataRow[0]["experience"];
        money = (int)dataRow[0]["money"];
    }

    protected virtual void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();

        if (!PhotonNetwork.IsMasterClient) return;

        Rotation();
    }

    // 플레이어와 충돌 후 AI velocity가 이상하게 적용되는 것 방지
    protected virtual void FreezeVelocity()
    {
        if (!isRush)
        {
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }

    // 타겟 설정
    private void UpdateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 50f, 1 << LayerMask.NameToLayer("Player"));

        if (cols.Length > 0)
        {
            float closestDistance = float.MaxValue;
            GameObject closestPlayer = null;

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].tag == "Player")
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, cols[i].transform.position);

                    if (distanceToPlayer < closestDistance)
                    {
                        closestDistance = distanceToPlayer;
                        closestPlayer = cols[i].gameObject;
                    }
                }
            }
            target = closestPlayer;
        }
        else
        {
            target = null;
            currHp = maxHp;
        }
    }

    protected virtual void Rotation()
    {
        if (isLookTarget && !isDie && target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            float rotationSpeed = 5f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
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

    protected abstract IEnumerator Think();

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.tag == "PlayerAttack")
        {
            if (!isDie)
            {
                OnDamage(other.gameObject.GetComponentInParent<PlayerStat>().realDmg);
            }
        }
        else if (other.tag == "SkillAttack")
        {
            if (!isDie)
            {
                OnDamage(other.GetComponentInParent<Skill_1>().damage);
            }
        }
    }

    private void OnDamage(float damage)
    {
        float modifiedDamage = Random.Range(damage * 0.85f, damage * 1.15f);
        modifiedDamage = Mathf.RoundToInt(modifiedDamage);

        photonView.RPC("ApplyColor", RpcTarget.All, 1);
        photonView.RPC("SpawnDamageText", RpcTarget.AllBuffered, transform.position, modifiedDamage);
        photonView.RPC("ApplyHp", RpcTarget.All, modifiedDamage);

        if (currHp <= 0)
        {
            Death();
        }
        StartCoroutine(ResetMaterial());
    }

    private IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.3f);
        photonView.RPC("ApplyColor", RpcTarget.All, 0);
    }

    private void Death()
    {
        photonView.RPC("DeathRPC", RpcTarget.AllBuffered);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doDie");

        StopAllCoroutines();

        photonView.RPC("RewardRPC", RpcTarget.AllBuffered); // 보상

        StartCoroutine(DestroyAfter(gameObject, 4f));
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

    protected virtual void SetRealDmgRPC(float damage)
    {
        realDmg = damage;
    }

    protected virtual void AnimTriggerRpc(string animStr)
    {
        if (anim != null)
            anim.SetTrigger(animStr);
    }

    protected virtual void AnimBoolRpc(string animStr, bool isBool)
    {
        if (anim != null)
            anim.SetBool(animStr, isBool);
    }

    protected virtual void ApplyColor(int colorNum)
    {
        mesh.material = mat[colorNum];
    }

    protected virtual void ApplyHp(float damage)
    {
        currHp -= (int)damage;

        hpBar.fillAmount = (float)currHp / maxHp;

        onDamageSource.Play();
    }

    protected virtual void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject text = Instantiate(damageText);
        position.y += 1.0f;
        text.transform.position = position;
        text.GetComponent<DamageText>().damage = damage;
    }

    protected virtual void DeathRPC()
    {
        isDie = true;
        gameObject.layer = 7;  // MonsterDead Layer
    }

    protected virtual void RewardRPC()
    {
        // 보상
        IngameManager.instance.stat.currHp = IngameManager.instance.stat.maxHp;
        IngameManager.instance.stat.currMp = IngameManager.instance.stat.maxMp;
        IngameManager.instance.stat.SetExp(exp);
        IngameManager.instance.stat.money += money;
    }
}
