using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class InvenData : SerializableDictionary<int, Items> { }

[System.Serializable]
public class Items
{
    public Item item;
    public int itemCount;
}

public class Inventory: MonoBehaviourPun
{
    //~~~~~~~~ 슬롯 크기~~~~~~~~~~
    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    private int slotCnt;
    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~`

    public delegate void OnChangeItem(int index);
    public OnChangeItem onChangeItem;

    public InvenData invenData;

    private void Start()
    {
        Debug.Log("Inventory");

        if (!photonView.IsMine) return;

        SystemManager.instance.dbManager.LoadCharacterDB();
        invenData = SystemManager.instance.dbManager.LoadInventory();

        // 인벤토리 관련 델리게이트 설정
        IngameManager.instance.uiManager.inventoryUI.SetDelegate();

        for (int i = -2; i < 40; i++)
        {
            onChangeItem.Invoke(i);
        }

        SlotCnt = 40;
    }

    public bool AddItem(Item _item, int count)
    {
        if (invenData.Count < SlotCnt)
        {
            // 카운트 가능한 아이템(ex. 포션)
            if (_item.itemType != ItemType.Null && _item.itemType != ItemType.equipment)
            {
                int keyNum = -999;
                foreach (KeyValuePair<int, Items> item in invenData)
                {
                    if (item.Value.item.itemCode == _item.itemCode)
                    {
                        keyNum = item.Key;
                        break;
                    }
                }

                // 이미 인벤토리에 있는 아이템일 경우
                if (keyNum != -999)
                {
                    invenData[keyNum].itemCount += count;

                    if (onChangeItem != null)
                        onChangeItem.Invoke(keyNum);

                    return true;
                }
                else   // 처음 먹는 아이템일 경우
                {
                    for (int i = 0; i < SlotCnt; i++)
                    {
                        if (!invenData.ContainsKey(i))
                        {
                            Items temp = new Items();
                            temp.itemCount = count;
                            temp.item = _item;
                            invenData.Add(i, temp);

                            if (onChangeItem != null)
                                onChangeItem.Invoke(i);

                            return true;
                        }
                    }
                }
            }
            else // 아이템 슬롯 당 1개의 아이템만 가질수 있는 아이템의 경우(ex. 장비) 
            {
                for (int i = 0; i < SlotCnt; i++)
                {
                    if (!invenData.ContainsKey(i))
                    {
                        Items temp = new Items();
                        temp.itemCount = count;
                        temp.item = _item;
                        invenData.Add(i, temp);

                        if (onChangeItem != null)
                            onChangeItem.Invoke(i);

                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void UseItem(int _index)
    {
        switch (invenData[_index].item.itemType)
        {
            case ItemType.Potion:
                if (invenData[_index].itemCount > 1)
                {
                    invenData[_index].itemCount--;
                }
                else
                {
                    invenData.Remove(_index);
                }
                break;
            case ItemType.equipment:
                invenData.Remove(_index);
                break;
        }
        onChangeItem.Invoke(_index);
    }

    public void RemoveItem(int _index)
    {
        invenData.Remove(_index);
        onChangeItem.Invoke(_index);
    }

    /*
    //아이템과 부딪히면 먹어지게 하기
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("FieldItem"))
        {
            FieldItems fieldItems = other.GetComponent<FieldItems>();
            if (AddItem(fieldItems.GetItem(), fieldItems.count))
                fieldItems.DestroyItem();
        }
    }
    */
}