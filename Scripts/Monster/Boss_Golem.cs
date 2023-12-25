using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss_Golem : Boss
{
    private bool isBreath = false;

    protected override IEnumerator Think()
    {
        while (target == null)
            yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(3f);

        int ranAction = Random.Range(0, 3);
        switch (ranAction)
        {
            case 0:
                Rush();
                break;
            case 1:
                Smash();
                break;
            case 2:
                Breath();
                break;
            default:
                break;
        }
    }

    private void Rush()
    {
        isRush = true;
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.0f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doRush");
    }

    public void RushForce()
    {
        rigid.AddForce(transform.forward * 15, ForceMode.Impulse);
    }

    public void EndRush()
    {
        isRush = false;
    }

    private void Smash()
    {
        isLookTarget = false;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 1.5f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doSmash");
    }

    private void Breath()
    {
        isLookTarget = true;
        photonView.RPC("SetRealDmgRPC", RpcTarget.AllBuffered, damage * 2.0f);
        photonView.RPC("AnimTriggerRpc", RpcTarget.AllBuffered, "doBreath");
        isBreath = true;
    }

    public void EndBreath()
    {
        isBreath = false;
    }

    public void StartThink()
    {
        isLookTarget = true;

        StartCoroutine(Think());
    }

    protected override void Rotation()
    {
        if (isLookTarget && !isDie && target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            float rotationSpeed = isBreath ? 1f : 5f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
        }
    }

    [PunRPC]
    protected override void SetRealDmgRPC(float damage)
    {
        base.SetRealDmgRPC(damage);
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
    protected override void AnimTriggerRpc(string animStr)
    {
        base.AnimTriggerRpc(animStr);
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

        IngameManager.instance.stat.skill_unlock[0] = "1";
        if (IngameManager.instance.questManager.questId == 30 && IngameManager.instance.questManager.questActionIndex == 1)
            IngameManager.instance.questManager.bossKill += 1;
    }
}
