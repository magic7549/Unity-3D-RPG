using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss_FlameDragon : Boss
{
    private int rushNum = 0;    // 돌진공격 연속 횟수

    protected override IEnumerator Think()
    {
        while (target == null)
            yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(Random.Range(2, 4));

        int ranAction = Random.Range(0, 9);
        switch (ranAction)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                Bite();
                break;
            case 4:
            case 5:
                Breath();
                break;
            case 6:
            case 7:
            case 8:
                Rush();
                break;
            default:
                break;
        }
    }

    private void Bite()
    {
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 0.8f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doBite");
    }

    private void Breath()
    {
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.3f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doBreath");
    }

    private void Rush()
    {
        if (target == null)
        {
            StartThink();
        }

        isRush = true;
        isLookTarget = false;
        rigid.velocity = Vector3.zero;
        rigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Collider>().enabled = false;

        rushNum++;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.0f);
        photonView.RPC("AnimBoolRpc", RpcTarget.AllBuffered, "isRush", true);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doRush");

        Vector3 lookTarget = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(lookTarget);

        Vector3 originalDirection = (target.transform.position - transform.position).normalized;
        Vector3 horizontalDirection = new Vector3(originalDirection.x, 0f, originalDirection.z).normalized;
        rigid.AddForce(horizontalDirection * 15, ForceMode.Impulse);
    }

    private void EndRush()
    {
        isRush = false;
        photonView.RPC("AnimBoolRpc", RpcTarget.AllBuffered, "isRush", false);
        GetComponent<Collider>().enabled = true;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        float additionalDashChance = 0.8f - (0.3f * rushNum);
        if (Random.value < additionalDashChance)
        {
            isLookTarget = true;
            Rush();
        }
        else
        {
            rushNum = 0;
            StartThink();
        }
    }

    public void StartThink()
    {
        isLookTarget = true;

        StartCoroutine(Think());
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
        IngameManager.instance.stat.skill_unlock[1] = "1";
        if (IngameManager.instance.questManager.questId == 60 && IngameManager.instance.questManager.questActionIndex == 1)
            IngameManager.instance.questManager.bossKill += 1;
    }
}
