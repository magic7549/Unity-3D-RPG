using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HornetTurtle : Monster
{
    [PunRPC]
    protected override void SetSpawner(string spawnerName)
    {
        base.SetSpawner(spawnerName);
    }

    [PunRPC]
    protected override void RewardRPC()
    {
        base.RewardRPC();
    }

    [PunRPC]
    protected override void DeathRPC()
    {
        base.DeathRPC();
    }

    [PunRPC]
    protected override void SpawnDamageText(Vector3 position, float damage)
    {
        base.SpawnDamageText(position, damage);
    }

    [PunRPC]
    protected override void ApplyHp(float damage)
    {
        base.ApplyHp(damage);
    }

    [PunRPC]
    protected override void AnimTriggerRpc(string animStr)
    {
        base.AnimTriggerRpc(animStr);
    }
}
