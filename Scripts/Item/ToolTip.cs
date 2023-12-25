using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI descText;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private TextMeshProUGUI useText;

    public void ShowToolTip(Vector2 pos, int itemCode)
    {
        panel.SetActive(true);

        // 툴팁 위치
        float _x = pos.x > (Screen.width / 2) ? -140f : 140f;
        float _y = pos.y > (Screen.height / 2) ? -110f : 110f;
        pos += new Vector2(_x, _y);
        panel.transform.position = pos;

        // 해당 아이템으로 텍스트 변경
        nameText.text = IngameManager.instance.itemDatabase.itemDB[itemCode].itemName;
        descText.text = IngameManager.instance.itemDatabase.itemDB[itemCode].itemDesc;
        priceText.text = "$" + IngameManager.instance.itemDatabase.itemDB[itemCode].itemPrice;
        switch (IngameManager.instance.itemDatabase.itemDB[itemCode].itemType)
        {
            case ItemType.equipment:
                useText.text = "장착";
                break;
            case ItemType.Potion:
                useText.text = "사용";
                break;
        }
    }

    public void HideToolTip()
    {
        panel.SetActive(false);

    }

    public void MoveToolTip(Vector2 pos)
    {
        float _x = pos.x > (Screen.width / 2) ? -140f : 140f;
        float _y = pos.y > (Screen.height / 2) ? -110f : 110f;
        pos += new Vector2(_x, _y);
        panel.transform.position = pos;
    }
}
