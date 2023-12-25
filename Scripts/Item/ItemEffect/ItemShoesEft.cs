using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Equipment/Shoes")]
public class ItemShoesEft : ItemEffect
{
    public int itemCode;
    public float speed;

    private int itemType = 3;

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

        // 스피드 증가 적용
        IngameManager.instance.stat.speed += speed;

        // 장비창 이미지 update
        IngameManager.instance.uiManager.equipmentUI.shoesSlot.GetComponent<EquipmentSlot>().UpdateImage();
        return true;
    }

    // 장착 해제
    public override void Unequip()
    {
        //스피드 감소 적용
        IngameManager.instance.stat.speed -= speed;
        IngameManager.instance.stat.speed = (int)Mathf.Clamp(IngameManager.instance.stat.speed, 1f, 1f);

        // 장비 아이템 코드값 0으로 변경
        IngameManager.instance.equipment.equipment[itemType] = 0;
    }
}