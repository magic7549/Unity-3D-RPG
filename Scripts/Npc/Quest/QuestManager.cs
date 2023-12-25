using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex;
    public int monsterKill;
    public int bossKill;

    public Dictionary<int, QuestNpc> questList;

    [SerializeField]
    private GameObject boatObject;
    [SerializeField]
    private GameObject carObject;
    [SerializeField]
    private GameObject shield;
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        questId = IngameManager.instance.stat.quest[0]; //퀘스트 번호
        questActionIndex = IngameManager.instance.stat.quest[1]; //퀘스트 진행 번호
        monsterKill = IngameManager.instance.stat.quest[2]; //죽인 몬스터 킬수
        bossKill = IngameManager.instance.stat.quest[3]; //죽인 보스 킬수
    }

    private void Awake()
    {
        questList = new Dictionary<int, QuestNpc>(); //초기화
        GenerateData();
        Debug.Log(questList[questId].questName);
    }

    void GenerateData()
    {
        questList.Add(10, new QuestNpc("이든에게 말걸기", new int[] { 1000, 2000 })); //연관된 npc번호 int[]
        questList.Add(20, new QuestNpc("몬스터 20마리 처치하기", new int[] { 2000, 2000, 2000 }));
        questList.Add(30, new QuestNpc("로봇 보스 처치하기", new int[] { 3000, 3000, 3000 }));
        questList.Add(40, new QuestNpc("외곽 황무지의\n마을 주민들에게 말걸기", new int[] { 4000, 5000 }));
        questList.Add(50, new QuestNpc("주변 몬스터 처치하기", new int[] { 5000, 5000, 5000 }));
        questList.Add(60, new QuestNpc("돌산 드래곤 처치하기", new int[] { 6000, 6000, 6000 }));
        questList.Add(70, new QuestNpc("인간A 만나기", new int[] { 8000, 8000, 8000 }));
        questList.Add(80, new QuestNpc("클리어", new int[] { 0 }));
    }

    public int GetQuestIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string TalkQuest(int id)
    {
        // 퀘스트 다음 액션 
        if (id == questList[questId].npcId[questActionIndex])
            Debug.Log("대화 퀘스트 : " + questActionIndex++);

        // 퀘스트 클리어 조건
        if (questActionIndex >= questList[questId].npcId.Length)
        {
            Debug.Log("대화 퀘스트 성공");
            NextQuest();
        }

        return questList[questId].questName;
    }


    //퀘스트 완료 시 보상
    void NextQuest()
    {

        switch (questId)
        {
            case 10:
                questId += 10;
                questActionIndex = 1; //10번 깨자마자 바로 20번 퀘스트 시작
                monsterKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 20:
                IngameManager.instance.stat.money += 100;
                questId += 10;
                questActionIndex = 0;
                monsterKill = 0;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 30:
                //배와 상호작용할 수 있도록 설정
                SphereCollider boatCollider = boatObject.GetComponent<SphereCollider>();
                boatCollider.enabled = true;
                
                questId += 10;
                questActionIndex = 0;
                monsterKill = 0;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 40:
                questId += 10;
                questActionIndex = 1;
                monsterKill = 0;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 50:
                IngameManager.instance.inventory.AddItem(IngameManager.instance.itemDatabase.itemDB[1], 15);
                IngameManager.instance.inventory.AddItem(IngameManager.instance.itemDatabase.itemDB[2], 5);

                questId += 10;
                questActionIndex = 0;
                monsterKill = 0;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 60:
                //차와 상호작용할 수 있도록 설정 
                SphereCollider carCollider = carObject.GetComponent<SphereCollider>();
                carCollider.enabled = true;

                questId += 10;
                questActionIndex = 1;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
            case 70:
                shield.SetActive(false);
                animator.SetTrigger("Last");
                IngameManager.instance.uiManager.endingUI.OpenEndingChoose();
                questId += 10;
                questActionIndex = 0;
                bossKill = 0;
                Debug.Log(questList[questId].questName);
                break;
        }

        SyncQuestInfo();
    }

    public void MonsterQuest(int id)
    {
        // 몬스터 20마리를 죽이면 퀘스트 완료
        if (monsterKill >= 20)
        {
            questActionIndex++;
            Debug.Log("몬스터 퀘스트 성공");

            SyncQuestInfo();
        }
    }

    public void BossQuest(int id)
    {
        // 보스 죽이면 퀘스트 완료
        if (bossKill >= 1)
        {
            questActionIndex++;
            Debug.Log("보스 퀘스트 성공");

            SyncQuestInfo();
        }

        // 클리어 조건
        if (questActionIndex >= questList[questId].npcId.Length)
        {
            Debug.Log("퀘스트 성공");
            NextQuest();
        }
    }

    public void SyncQuestInfo()
    {
        IngameManager.instance.stat.quest[0] = questId;
        IngameManager.instance.stat.quest[1] = questActionIndex;
        IngameManager.instance.stat.quest[2] = monsterKill;
        IngameManager.instance.stat.quest[3] = bossKill;
    }
}


