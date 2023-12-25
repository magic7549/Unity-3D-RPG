using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Equipment/Shield")]
public class ItemShieldEft : ItemEffect
{
    public override bool ExecuteRole()
    {
        //PlayerStatus.instance.equipment[1].SetActive(true);
        return true;
    }
}

