using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Boss_ExperimentalX7Cain : Boss
{
    private NavMeshAgent nav;
    [SerializeField] private GameObject buffParticleObject;
    private bool isAttack = false;
    private bool isBattlecry = false;

    protected override void Start()
    {
        base.Start();

        if (!PhotonNetwork.IsMasterClient) return;

        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
    }

    protected override void Update()
    {
        base.Update();

        if (PhotonNetwork.IsMasterClient && nav.enabled && target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // 플레이어에 충분히 가까워졌을 때 SetDestination 해제
            if (distanceToTarget <= 2.5f)
            {
                nav.isStopped = true;
                anim.SetBool("isWalk", false);
            }
            else
            {
                // 목적지까지 도달할 수 있는지 확인
                if (nav.CalculatePath(target.transform.position, new NavMeshPath()))
                {
                    nav.isStopped = isAttack;
                    anim.SetBool("isWalk", !isAttack);
                    nav.SetDestination(target.transform.position);
                }
                else
                {
                    // 도달할 수 없는 경우 SetDestination 해제
                    nav.isStopped = true;
                    anim.SetBool("isWalk", false);
                }
            }
        }
    }

    protected override void FreezeVelocity()
    {
        // FreezeVelocity() 해제
    }

    protected override IEnumerator Think()
    {
        while (target == null)
            yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(Random.Range(0, 5));

        int ranAction = Random.Range(0, 2);
        switch (ranAction)
        {
            case 0:
                JumpAttack();
                break;
            case 1:
                Swiping();
                break;
            default:
                break;
        }
    }

    public void StartThink()
    {
        isLookTarget = true;
        isAttack = false;

        StartCoroutine(Think());
    }

    private void JumpAttack()
    {
        isAttack = true;
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.3f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doJumpAttack");
    }

    private void Swiping()
    {
        isAttack = true;
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.0f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doSwiping");
    }

    [PunRPC]
    protected override void SetRealDmgRPC(float damage)
    {
        base.SetRealDmgRPC(damage);
    }

    [PunRPC]
    protected override void AnimTriggerRpc(string animStr)
    {
        base.AnimTriggerRpc(animStr);
    }

    [PunRPC]
    protected override void AnimBoolRpc(string animStr, bool isBool)
    {
        base.AnimBoolRpc(animStr, isBool);
    }

    [PunRPC]
    protected override void ApplyColor(int colorNum)
    {
        base.ApplyColor(colorNum);
    }

    [PunRPC]
    protected override void ApplyHp(float damage)
    {
        base.ApplyHp(damage);

        // 버프 활성화
        if (currHp < (maxHp * 0.5f) && !isBattlecry)
        {
            isBattlecry = true;

            if (PhotonNetwork.IsMasterClient)
            {
                StopAllCoroutines();
                isAttack = true;
                isLookTarget = false;

                photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doBattlecry");
            }

            photonView.RPC("SetDamageRPC", RpcTarget.AllBuffered, this.damage * 1.3f);
            currHp += Mathf.RoundToInt(maxHp * 0.2f);
            hpBar.fillAmount = (float)currHp / maxHp;
        }
    }

    public void OnBuffEffect()
    {
        photonView.RPC("OnBuffEffectRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetDamageRPC(float damage)
    {
        this.damage = damage;
    }

    [PunRPC]
    private void OnBuffEffectRPC()
    {
        buffParticleObject.SetActive(true);
    }

    [PunRPC]
    protected override void SpawnDamageText(Vector3 position, float damage)
    {
        base.SpawnDamageText(position, damage);
    }

    [PunRPC]
    protected override void DeathRPC()
    {
        base.DeathRPC();
    }

    [PunRPC]
    protected override void RewardRPC()
    {
        base.RewardRPC();
        if (IngameManager.instance.questManager.questId == 70 && IngameManager.instance.questManager.questActionIndex == 2)
            IngameManager.instance.questManager.bossKill += 1;
    }
}
