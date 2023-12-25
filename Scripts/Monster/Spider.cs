using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spider : Monster
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

        // 몬스터 처치 퀘스트 확인
        if (IngameManager.instance.questManager.questId == 20 && IngameManager.instance.questManager.questActionIndex == 1)
            IngameManager.instance.questManager.monsterKill += 1;
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
