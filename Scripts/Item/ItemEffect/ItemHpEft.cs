using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Health")]
public class ItemHpEft : ItemEffect
{
    public int healingPoint = 80;

    public override bool ExecuteRole()
    {
        if (IngameManager.instance.stat.currHp >= IngameManager.instance.stat.maxHp)
        {
            Debug.Log("체력이 최대입니다.");
            return false;
        }
        else
        {
            Debug.Log("Add : " + healingPoint);
            IngameManager.instance.stat.currHp = (int)Mathf.Clamp(IngameManager.instance.stat.currHp + healingPoint, 0f, IngameManager.instance.stat.maxHp);
            return true;
        }
    }

}
