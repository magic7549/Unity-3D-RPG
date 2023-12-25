using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler
{
    [SerializeField]
    private Image itemIcone;
    [SerializeField]
    private TextMeshProUGUI itemCountText;
    private Item item;

    public void SetItem(Item item)
    {
        itemIcone.gameObject.SetActive(true);
        itemCountText.gameObject.SetActive(true);

        this.item = item;
        itemIcone.sprite = item.itemImage;
        itemCountText.text = "1";
    }

    public void ClearItem()
    {
        itemIcone.gameObject.SetActive(false);
        itemCountText.gameObject.SetActive(false);

        item = null;
        itemIcone.sprite = null;
        itemCountText.text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 사운드
        SystemManager.instance.soundManager.PlaySound(0);
    }

    public void OnPointerClick(PointerEventData eventData) //사용 
    {
        // 마우스 좌클릭(구매)
        if (item != null && eventData.button == PointerEventData.InputButton.Left)
        {
            IngameManager.instance.uiManager.shopUI.ClickItem(item);
        }
    }

    // 마우스 커서가 슬롯 위로 올라갈 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            IngameManager.instance.uiManager.toolTipUI.ShowToolTip(eventData.position, item.itemCode);
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
        if (item != null)
        {
            IngameManager.instance.uiManager.toolTipUI.MoveToolTip(eventData.position);
        }
    }
}
