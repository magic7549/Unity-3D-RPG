using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EquipmentSlotName
{
    weapon,
    necklace,
    ring,
    shoes
}
public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] private EquipmentSlotName equipmentSlotName;
    [SerializeField] private Image image;

    public void UpdateImage()
    {
        Debug.Log("UpdateImage");

        image.sprite = IngameManager.instance.itemDatabase.itemDB[IngameManager.instance.equipment.equipment[(int)equipmentSlotName]].itemImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 장비를 장착중일때만 ToolTip 표시
        if (IngameManager.instance.equipment.equipment[(int)equipmentSlotName] != 0)
        {
            IngameManager.instance.uiManager.toolTipUI.ShowToolTip(eventData.position, IngameManager.instance.equipment.equipment[(int)equipmentSlotName]);
            Cursor.visible = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IngameManager.instance.uiManager.toolTipUI.HideToolTip();
        Cursor.visible = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (IngameManager.instance.equipment.equipment[(int)equipmentSlotName] != 0)
        {
            IngameManager.instance.uiManager.toolTipUI.MoveToolTip(eventData.position);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 사운드
        SystemManager.instance.soundManager.PlaySound(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 우클릭
        if (eventData.button == PointerEventData.InputButton.Right && IngameManager.instance.equipment.equipment[(int)equipmentSlotName] != 0)
        {
            // 장착중인 아이템을 인벤토리로 추가
            IngameManager.instance.inventory.AddItem(IngameManager.instance.itemDatabase.itemDB[IngameManager.instance.equipment.equipment[(int)equipmentSlotName]], 1);

            // 증가했던 스탯 복구
            IngameManager.instance.itemDatabase.itemDB[IngameManager.instance.equipment.equipment[(int)equipmentSlotName]].efts.Unequip();

            // 툴팁 숨기기
            IngameManager.instance.uiManager.toolTipUI.HideToolTip();
            Cursor.visible = true;

            // 이미지 업데이트
            UpdateImage();
        }
    }

    private void OnDisable()
    {
        IngameManager.instance.uiManager.toolTipUI.HideToolTip();
        Cursor.visible = true;
    }
}
