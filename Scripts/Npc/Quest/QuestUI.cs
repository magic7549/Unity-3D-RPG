using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private int questid;
    
    public void OpenQuestPanel(int id)
    {
        questid = id;
        gameObject.SetActive(true);
        IngameManager.instance.playerController.OpenMenu(); // 캐릭터 움직임 멈춤
    }

    // 수락 버튼 클릭
    public void OnClickQuestAccept()
    {
        IngameManager.instance.questManager.TalkQuest(questid);
        gameObject.SetActive(false);
        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }

    // 거절 버튼 클릭
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }
}