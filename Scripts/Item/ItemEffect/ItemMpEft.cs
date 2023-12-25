using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Mana")]
public class ItemMpEft : ItemEffect
{
    public int ManaPoint = 30;

    public override bool ExecuteRole()
    {
        if (IngameManager.instance.stat.currMp >= IngameManager.instance.stat.maxMp)
        {
            Debug.Log("최대 마나입니다.");
            return false;
        }
        else
        {
            Debug.Log("Add : " + ManaPoint);
            IngameManager.instance.stat.currMp = (int)Mathf.Clamp(IngameManager.instance.stat.currMp + ManaPoint, 0f, IngameManager.instance.stat.maxMp);
            return true;
        }
    }
}
