using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject noMoneyPanel;
    [SerializeField] private GameObject completePanel;

    private ShopSlot[] slots;
    private Item item;

    private void Start()
    {
        slots = shopPanel.GetComponentsInChildren<ShopSlot>();
    }

    public void OpenShop(int[] itemCode)
    {
        shopPanel.SetActive(true);

        // 판매할 아이템 슬롯에 할당
        for (int i = 0; i < itemCode.Length; i++)
        {
            slots[i].SetItem(IngameManager.instance.itemDatabase.itemDB[itemCode[i]]);
        }

        // 마우스 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 인벤토리 열리면 카메라 회전 멈춤
        IngameManager.instance.cameraMovement.RotationControl(false);
    }

    public void CloseShop()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearItem();
        }

        shopPanel.SetActive(false);
        confirmPanel.SetActive(false);
        noMoneyPanel.SetActive(false);
        completePanel.SetActive(false);

        // 마우스 비활성화
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 인벤토리 열리면 카메라 회전
        IngameManager.instance.cameraMovement.RotationControl(true);
    }

    public void ClickItem(Item item)
    {
        this.item = item;
        confirmPanel.SetActive(true);
    }

    // 아이템 구매 버튼 클릭
    public void OnClickPurchase()
    {
        confirmPanel.SetActive(false);

        // 소지금 >= 아이템 가격
        if (item.itemPrice <= IngameManager.instance.stat.money)
        {
            IngameManager.instance.stat.money -= item.itemPrice;
            IngameManager.instance.inventory.AddItem(item, 1);

            completePanel.SetActive(true);
        }
        else
        {
            noMoneyPanel.SetActive(true);
        }
    }
}
