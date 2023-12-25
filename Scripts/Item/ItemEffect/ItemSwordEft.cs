using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Equipment/Sword")]
public class ItemSwordEft : ItemEffect
{
    public int itemCode;
    public float damage;

    private int itemType = 0;

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

        // 무기 데미지 적용
        IngameManager.instance.stat.damage += damage;

        // 장비창 이미지 update
        IngameManager.instance.uiManager.equipmentUI.weaponSlot.GetComponent<EquipmentSlot>().UpdateImage();

        // 장비 모델링 변경
        IngameManager.instance.weapon.ChangeWeapon();
        return true;
    }

    // 장착 해제
    public override void Unequip()
    {
        // 데미지 감소
        IngameManager.instance.stat.damage -= damage;

        // 장비 아이템 코드값 0으로 변경
        IngameManager.instance.equipment.equipment[itemType] = 0;

        // 장비 모델링 변경
        IngameManager.instance.weapon.ChangeWeapon();
    }
}

