using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterInfo_Text : MonoBehaviour
{
    private TextMeshProUGUI text; 

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>(); 

        int slotNum = int.Parse(gameObject.name.Substring(9, 1)) - 1;
        if (SystemManager.instance.dbManager.slot[slotNum] != 0)
        {
            string temp = "slot" + (slotNum + 1);
            for (int i = 0; i < SystemManager.instance.dbManager.userTable.Rows.Count; i++)
            {
                if (SystemManager.instance.dbManager.userTable.Rows[i]["slot"].ToString().Equals(temp))
                {
                    text.text =
                        "레벨   : " + SystemManager.instance.dbManager.userTable.Rows[i]["level"].ToString() + "\n" +
                        "소지금 : " + SystemManager.instance.dbManager.userTable.Rows[i]["money"].ToString() + "\n" +
                        "최대 체력 : " + SystemManager.instance.dbManager.userTable.Rows[i]["maxHp"].ToString() + "\n" +
                        "현재 체력 : " + SystemManager.instance.dbManager.userTable.Rows[i]["currHp"].ToString() + "\n" +
                        "최대 마나 : " + SystemManager.instance.dbManager.userTable.Rows[i]["maxMp"].ToString() + "\n" +
                        "현재 마나 : " + SystemManager.instance.dbManager.userTable.Rows[i]["currMp"].ToString() + "\n" +
                        "데미지 : " + SystemManager.instance.dbManager.userTable.Rows[i]["damage"].ToString();
                }
            }
        }
        else
        {
            text.text = "캐릭터 없음";
        }
    }
}