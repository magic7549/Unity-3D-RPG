using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Equipment/Ring")]
public class ItemRingEft : ItemEffect
{
    public int itemCode;
    public int mp;

    private int itemType = 2;

    public override bool ExecuteRole()
    {
        if (IngameManager.instance.equipment.equipment[itemType] != 0)
        {
            // 장착중인 아이템을 인벤토리로 추가
            IngameManager.instance.inventory.AddItem(IngameManager.instance.itemDatabase.itemDB[IngameManager.instance.equipment.equipment[itemType]], 1);
            // 장착중인 아이템 효과 되돌리기
            IngameManager.instance.itemDatabase.itemDB[IngameManager.instance.equipment.equipment[itemType]].efts.Unequip();
            // 새로 장착할 아이템을 장비로
            IngameManager.instance.equipment.equipment[itemType] = itemCode;
        }
        // 장비 칸이 비어있을 경우
        else
        {
            IngameManager.instance.equipment.equipment[itemType] = itemCode;
        }

        // 최대 MP 적용
        IngameManager.instance.stat.maxMp += mp;

        // MP 바 최대량 증가
        IngameManager.instance.uiManager.playerBar.UpdateMax();

        // 장비창 이미지 update
        IngameManager.instance.uiManager.equipmentUI.ringSlot.GetComponent<EquipmentSlot>().UpdateImage();
        return true;
    }

    // 장착 해제
    public override void Unequip()
    {
        // 최대 MP 적용 해제
        IngameManager.instance.stat.maxMp -= mp;
        IngameManager.instance.stat.currMp = (int)Mathf.Clamp(IngameManager.instance.stat.currMp, 0f, IngameManager.instance.stat.maxMp);

        // MP 바 최대량 감소
        IngameManager.instance.uiManager.playerBar.UpdateMax();

        // 장비 아이템 코드값 0으로 변경
        IngameManager.instance.equipment.equipment[itemType] = 0;
    }
}