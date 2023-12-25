using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Photon.Pun;

public class PlayerStat : MonoBehaviourPun
{
    public int level;
    public int exp;
    public int requiredExp;
    public int money;
    public int maxHp;
    public int currHp;
    public int maxMp;
    public int currMp;
    public float damage;
    public float realDmg;
    public float speed;

    public int[] quest;
    public string[] skill_unlock;
    public string[] savepoint_unlock;

    // 마지막으로 이용한 세이브 포인트 번호
    public int lastUseSavepoint;

    private void Awake()
    {
        if (!photonView.IsMine) return;

        SystemManager.instance.dbManager.LoadCharacterDB();
        LoadData();

        photonView.RPC("SyncPlayerStats", RpcTarget.Others,
                level, exp, money, maxHp, currHp, maxMp, currMp, damage, realDmg, speed,
                quest, skill_unlock, savepoint_unlock, lastUseSavepoint);
    }

    private void LoadData()
    {
        string slot_str = "slot" + SystemManager.instance.selectSlotNum;

        for (int i = 0; i < SystemManager.instance.dbManager.userTable.Rows.Count; i++)
        {
            if (SystemManager.instance.dbManager.userTable.Rows[i]["slot"].ToString().Equals(slot_str))
            {
                level = (int)SystemManager.instance.dbManager.userTable.Rows[i]["level"];
                exp = (int)SystemManager.instance.dbManager.userTable.Rows[i]["exp"];
                money = (int)SystemManager.instance.dbManager.userTable.Rows[i]["money"];
                maxHp = (int)SystemManager.instance.dbManager.userTable.Rows[i]["maxHp"];
                currHp = (int)SystemManager.instance.dbManager.userTable.Rows[i]["currHp"];
                maxMp = (int)SystemManager.instance.dbManager.userTable.Rows[i]["maxMp"];
                currMp = (int)SystemManager.instance.dbManager.userTable.Rows[i]["currMp"];
                damage = (float)SystemManager.instance.dbManager.userTable.Rows[i]["damage"];
                speed = (float)SystemManager.instance.dbManager.userTable.Rows[i]["speed"];
                lastUseSavepoint = (int)SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"];

                // 퀘스트 배열
                string temp = (string)SystemManager.instance.dbManager.userTable.Rows[i]["quest"];
                temp = temp.Substring(1, temp.Length - 2);
                string[] tempArray = temp.Split(',');
                quest = new int[tempArray.Length];
                for (int j = 0; j < tempArray.Length; j++)
                {
                    quest[j] = int.Parse(tempArray[j]);
                }

                // 문자열로 이루어진 리스트 분해 ex) [1, 0] => skill_unlock[0] = 1, skill_unlock[1] = 0
                // 스킬 언락
                temp = (string)SystemManager.instance.dbManager.userTable.Rows[i]["skill_unlock"];
                temp = temp.Substring(1, temp.Length - 2);
                skill_unlock = temp.Split(", ");

                // 세이브 포인트 언락
                temp = (string)SystemManager.instance.dbManager.userTable.Rows[i]["savepoint_unlock"];
                temp = temp.Substring(1, temp.Length - 2);
                savepoint_unlock = temp.Split(", ");

                requiredExp = SystemManager.instance.dbManager.expTable.Select("level = " + level)[0].Field<int>("experience");
            }
        }
    }

    [PunRPC]
    private void SyncPlayerStats(int syncedLevel, int syncedExp, int syncedMoney, int syncedMaxHp, int syncedCurrHp,
                                  int syncedMaxMp, int syncedCurrMp, float syncedDamage, float syncedRealDmg,
                                  float syncedSpeed, int[] syncedQuest, string[] syncedSkillUnlock,
                                  string[] syncedSavepointUnlock, int syncedLastUseSavepoint)
    {
        level = syncedLevel;
        exp = syncedExp;
        money = syncedMoney;
        maxHp = syncedMaxHp;
        currHp = syncedCurrHp;
        maxMp = syncedMaxMp;
        currMp = syncedCurrMp;
        damage = syncedDamage;
        realDmg = syncedRealDmg;
        speed = syncedSpeed;
        quest = syncedQuest;
        skill_unlock = syncedSkillUnlock;
        savepoint_unlock = syncedSavepointUnlock;
        lastUseSavepoint = syncedLastUseSavepoint;
    }

    public void SetExp(int addExp)
    {
        if (level >= 20) return;

        exp += addExp;

        while (exp >= requiredExp)
        {
            exp -= requiredExp;
            level++;
            IngameManager.instance.uiManager.playerBar.UpdateLevelText();
            IngameManager.instance.playerController.LevelUp();

            int hp_increase = SystemManager.instance.dbManager.levelTable.Select("level = " + level)[0].Field<int>("hp") - SystemManager.instance.dbManager.levelTable.Select("level = " + (level - 1))[0].Field<int>("hp");
            maxHp += hp_increase;
            currHp = maxHp;
            currMp = maxMp;

            float dmg_increase = SystemManager.instance.dbManager.levelTable.Select("level = " + level)[0].Field<float>("damage") - SystemManager.instance.dbManager.levelTable.Select("level = " + (level - 1))[0].Field<float>("damage");
            damage += dmg_increase;

            // 다음 레벨 데이터 가져오기
            DataRow nextLevelData = SystemManager.instance.dbManager.expTable.Select("level = " + level)[0];
            if (nextLevelData != null)
            {
                requiredExp = nextLevelData.Field<int>("experience");
                IngameManager.instance.uiManager.playerBar.UpdateExpMax();
            }
            else
            {
                break;
            }
        }
    }
}
