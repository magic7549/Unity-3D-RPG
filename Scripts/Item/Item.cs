using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Null,
    Potion,
    equipment
}
// 직렬화를 하여야 ItemDatabase 인스펙터창에서 추가 가능
[System.Serializable]
public class Item
{
    public int itemCode;
    public ItemType itemType;
    public string itemName;
    public string itemDesc;
    public int itemPrice;
    public Sprite itemImage;
    public ItemEffect efts;

    public bool Use()
    {
        if (efts != null)
        {
            bool isUsed = efts.ExecuteRole();
            return isUsed;
        }

        return false;
    }

}
