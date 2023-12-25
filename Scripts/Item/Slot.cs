using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; //사용


public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler
{
    public int slotnum; // InventoryUI.cs에서 할당
    public bool isQuickSlot = false;

    [SerializeField]
    private Image itemIcone;
    [SerializeField]
    private TextMeshProUGUI itemCountText;
    [SerializeField]
    private Button removeButton;

    private Items items;
    private DragSlot dragSlot;
    private bool isDrag = false;

    private void Start()
    {
        dragSlot = IngameManager.instance.uiManager.inventoryUI.dragSlot;
    }

    public void UpdateSlotUI(Items newItem)
    {
        items = newItem;

        itemIcone.sprite = items.item.itemImage;
        itemCountText.text = items.itemCount.ToString();
       
        itemIcone.gameObject.SetActive(true);
        itemCountText.gameObject.SetActive(true);
        if (!isQuickSlot)
            removeButton.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        items = null;
        itemIcone.gameObject.SetActive(false);
        itemCountText.gameObject.SetActive(false);
        if (!isQuickSlot)
            removeButton.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 사운드
        SystemManager.instance.soundManager.PlaySound(0);
    }

    public void OnPointerClick(PointerEventData eventData) //사용 
    {
        // 마우스 우클릭
        if (eventData.button == PointerEventData.InputButton.Right) 
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (items != null)
        {
            bool isUse = items.item.Use();
            if (isUse)
            {
                if (items.item.itemType.Equals(ItemType.equipment))
                {
                    IngameManager.instance.uiManager.toolTipUI.HideToolTip();
                    Cursor.visible = true;
                }

                IngameManager.instance.inventory.UseItem(slotnum);
            }
        }
    }

    public void OnRemoveButtonClick()
    {
        IngameManager.instance.inventory.RemoveItem(slotnum);
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 마우스 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left) 
        {
            if (items != null && items.itemCount > 0)
            {
                isDrag = true;
                dragSlot.gameObject.SetActive(true);
                dragSlot.slot = this;
                dragSlot.DragSetImage(itemIcone);
                dragSlot.transform.position = eventData.position;
            }
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
            dragSlot.transform.position = eventData.position;
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            dragSlot.SetColor(0);
            dragSlot.slot = null;
            dragSlot.gameObject.SetActive(false);
            isDrag = false;
        }
    }

    // 해당 슬롯에 무언가가 마우스 드롭 됐을 때 발생하는 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (dragSlot.slot != null)
            ChangeSlot();
    }

    // 아이템 스왑
    private void ChangeSlot()
    {
        Items _tempItems = (items == null || items.item.itemType == ItemType.Null) ? null : items;
        UpdateSlotUI(dragSlot.slot.items);

        if (_tempItems != null)
        {
            dragSlot.slot.UpdateSlotUI(_tempItems);

            // 인벤토리 데이터 스왑
            IngameManager.instance.inventory.invenData[slotnum] = items;
            IngameManager.instance.inventory.invenData[dragSlot.slot.slotnum] = dragSlot.slot.items;
        }
        else
        {
            dragSlot.slot.RemoveSlot();

            IngameManager.instance.inventory.invenData[slotnum] = items;
            IngameManager.instance.inventory.RemoveItem(dragSlot.slot.slotnum);
        }
    }

    // 마우스 커서가 슬롯 위로 올라갈 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (items != null && items.itemCount > 0)
        {
            IngameManager.instance.uiManager.toolTipUI.ShowToolTip(eventData.position, items.item.itemCode);
            Cursor.visible = false;
        }
    }

    // 마우스 커서가 슬롯에서 나올 때
    public void OnPointerExit(PointerEventData eventData)
    {
        IngameManager.instance.uiManager.toolTipUI.HideToolTip();
        Cursor.visible = true;
    }

    // 마우스 커서가 슬롯위에서 움직일 때
    public void OnPointerMove(PointerEventData eventData)
    {
        if (items != null && items.itemCount > 0)
        {
            IngameManager.instance.uiManager.toolTipUI.MoveToolTip(eventData.position);
        }
    }

    private void OnDisable()
    {
        IngameManager.instance.uiManager.toolTipUI.HideToolTip();
        Cursor.visible = true;
    }
}
