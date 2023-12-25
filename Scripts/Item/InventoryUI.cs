using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    public GameObject inventoryPanel;
    [SerializeField]
    private GameObject moneyPanel;
    [SerializeField]
    private GameObject quickSlotPanel;
    [SerializeField]
    private Transform slotHolder;
    private bool activeInventory = false;
    private Slot[] slots;
    private Slot[] quickSlots;

    public DragSlot dragSlot;


    //아이템 없애기 버튼
    //public Toggle EnableRemove;

    private void Start()
    {
        slots = slotHolder.GetComponentsInChildren<Slot>();
        quickSlots = quickSlotPanel.GetComponentsInChildren<Slot>();

        inventoryPanel.SetActive(activeInventory); //인벤토리 나타나게 하기
        dragSlot.gameObject.SetActive(false);
    }

    public void SetDelegate()
    {
        IngameManager.instance.inventory.onSlotCountChange += SlotChange;
        IngameManager.instance.inventory.onChangeItem += RedrawSlotUI; //아이템 나타나게하기
    }

    public void RevokeDelegate()
    {
        IngameManager.instance.inventory.onSlotCountChange -= SlotChange;
        IngameManager.instance.inventory.onChangeItem -= RedrawSlotUI;
    }

    //슬롯 체인지 갯수만큼만 활성화하고 나머지는 비활성화
    private void SlotChange(int val) //사용
    {
        for (int i =0; i < val; i++)
        {
            slots[i].slotnum = i;
            if (i< IngameManager.instance.inventory.SlotCnt)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }

    public void AddSlot()
    {
        IngameManager.instance.inventory.SlotCnt += 5;
    }

    public void OpenAndCloseInven()
    {
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);
        moneyPanel.SetActive(activeInventory);
        Cursor.visible = activeInventory;
        Cursor.lockState = activeInventory ? CursorLockMode.None : CursorLockMode.Locked;

        // 인벤토리 열리면 카메라 회전 멈춤
        IngameManager.instance.cameraMovement.RotationControl(!activeInventory);
    }

    void RedrawSlotUI(int index)
    {
        if (IngameManager.instance.inventory.invenData.ContainsKey(index))
        {
            if (index >= 0)
                slots[index].UpdateSlotUI(IngameManager.instance.inventory.invenData[index]);
            else
            {
                quickSlots[(index * -1) - 1].UpdateSlotUI(IngameManager.instance.inventory.invenData[index]);
            }
        }
        else
        {
            if (index >= 0) 
                slots[index].RemoveSlot();
            else
            {
                quickSlots[(index * -1) - 1].RemoveSlot();
            }
        }
    }
    /*
    public void EnableItemsRemove()
    {
        if (EnableRemove.isOn)
        {
            for (int i = 0; i < inven.items.Count; i++)
            {
                slots[i].transform.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < inven.items.Count; i++)
            {
                slots[i].transform.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }
    */

}
