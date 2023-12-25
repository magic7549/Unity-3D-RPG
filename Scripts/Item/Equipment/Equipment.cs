using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Equipment : MonoBehaviourPun
{
    // 현재 끼고 있는 장비 아이템 코드 목록 (0: 무기, 1: 목걸이, 2: 반지, 3: 신발)
    public int[] equipment;     // 저장할때 문자열로 변환

    void Start()
    {
        Debug.Log("Equipment");

        if (!photonView.IsMine) return;

        // 장비 load
        SystemManager.instance.dbManager.LoadCharacterDB();
        equipment = SystemManager.instance.dbManager.LoadEquipment();

        // 장비창 이미지 update
        IngameManager.instance.uiManager.equipmentUI.weaponSlot.GetComponent<EquipmentSlot>().UpdateImage();
        IngameManager.instance.uiManager.equipmentUI.necklaceSlot.GetComponent<EquipmentSlot>().UpdateImage();
        IngameManager.instance.uiManager.equipmentUI.ringSlot.GetComponent<EquipmentSlot>().UpdateImage();
        IngameManager.instance.uiManager.equipmentUI.shoesSlot.GetComponent<EquipmentSlot>().UpdateImage();
    }
}
