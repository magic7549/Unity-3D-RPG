using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestInfoUI : MonoBehaviour
{
    private int questid;
    private int questindex;

    [SerializeField]
    private TextMeshProUGUI questNameText;
    [SerializeField]
    private TextMeshProUGUI questMarkText;

    private void Update()
    {
        questid = IngameManager.instance.questManager.questId;
        questindex = IngameManager.instance.questManager.questActionIndex;

        if (questid == 10)
        {
            questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
            questMarkText.text = ""; // 10번 퀘스트에 대한 추가 정보가 없으므로 비움
            if (questindex == 1)
            {
                questNameText.text = "";
            }
        }
        else if (questid == 20)
        {
            questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
            if (questindex == 1)
            {
                questMarkText.text = "주변 몬스터 " + IngameManager.instance.questManager.monsterKill + " / 20";
            }
            else
            {
                questNameText.text = "";
                questMarkText.text = ""; // 20번 퀘스트에 대한 추가 정보가 없거나 questindex가 1이 아닐 때 비움
            }
        }
        else if (questid == 30)
        {
            questNameText.text = "";
            if (questindex == 1)
            {
                questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
                questMarkText.text = "로봇 " + IngameManager.instance.questManager.bossKill + " / 1";
            }
            else
            {
                questMarkText.text = ""; // 30번 퀘스트에 대한 추가 정보가 없거나 questindex가 1이 아닐 때 비움
            }
        }
        else if (questid == 40)
        {
            questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
            questMarkText.text = "";
            if (questindex == 1)
            {
                questNameText.text = "";
            }
        }
        else if (questid == 50)
        {
            questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
            if (questindex == 1)
            {
                questMarkText.text = "주변 몬스터 " + IngameManager.instance.questManager.monsterKill + " / 20";
            }
            else
            {
                questNameText.text = "";
                questMarkText.text = ""; 
            }
        }
        else if (questid == 60)
        {
            questNameText.text = "";
            if (questindex == 1)
            {
                questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
                questMarkText.text = "드래곤 " + IngameManager.instance.questManager.bossKill + " / 1";
            }
            else
            {
                questMarkText.text = ""; 
            }
        }
        else if (questid == 70)
        {
            questNameText.text = "";
            questMarkText.text = "";
            if (questindex == 1)
            {
                questNameText.text = IngameManager.instance.questManager.questList[questid].questName;
            }
            else if (questindex == 2)
            {
                questNameText.text = "마지막 보스 처치하기";
                questMarkText.text = "인간 실험체 " + IngameManager.instance.questManager.bossKill + " / 1";
            }
            else
            {
                questMarkText.text = "";
            }
        }
        else if (questid == 80)
        {
            questNameText.text = "";
            if (questindex == 1)
            {
                questNameText.text = "인간A와 대화하기";
            }
            else if (questindex == 2)
            {
                questNameText.text = "인간A에게 마지막으로 말하기";
            }
            else
            {
                questNameText.text = "Game Clear!";
                questMarkText.text = "";
            }
        }
        else
        {
            questNameText.text = "";
            questMarkText.text = "";
        }
    }
}
