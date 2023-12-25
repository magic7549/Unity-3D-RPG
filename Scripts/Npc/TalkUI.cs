using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System;
using WebSocketSharp;
using Unity.VisualScripting;
using Photon.Pun.Demo.PunBasics;

public class TalkUI : MonoBehaviour
{
    [SerializeField]
    private GameObject talkObject;  // 대화창 배경(배경 오브젝트 하위에 텍스트 및 버튼 포함)
    [SerializeField]
    private GameObject nameObject;  // NPC 이름 텍스트
    [SerializeField]
    private GameObject conversationObject;  // 대화 내용 텍스트
    [SerializeField]
    private GameObject nextObject;  // 다음 or 닫기 버튼 텍스트
    [SerializeField]
    private Boss_Spawn boss_Spawn; // 보스 스폰하기

    private GameObject scanObject;
    private Dictionary<int, string[]> talkData = new Dictionary<int, string[]>(); // 대화 내용 <npc id, 대화 문장>
    public bool isConvo;    // 대화 중인지

    private int talkIndex = 0;

    private void Start()
    {
        talkObject.SetActive(false);

        MakeData();
    }

    // 대화 생성
    private void MakeData()
    {
        //이든
        talkData.Add(10 + 1000, new string[] { "여긴 어디지..? " +"\n" + "뭐야..? 내몸이 왜이래??"+"\n"+"몸이 모두 검정색으로 물들어 있잖아..!!!"
            , "오 깨어났네?"
            , "넌 누구야?! 여긴 어디지?"
            , "난 이 동굴에서 살고 있는 이든이라고 한다. "+"\n"+"괴롭겠지만 넌 그림자 속에서 태어난 괴물이야."+"\n"+"이곳에선 이상하게 가끔씩 너같은 얘들이 태어나지."
            , "무슨 소리야? 난 엄연히 사람이라고!"
            , "그럼 너의 이름은? 너의 가족들은 누구지?"
            , "내 이름은 R112..."+"\n"+".."+"\n"+"분명 가족이 있었지만..."+"\n"+"이름 말고는 아무것도 기억이 나지 않아.. 가족까지도."
            , "넌 그림자의 속성을 가지고 태어난 괴물일 뿐이다. "+"\n"+"여기에 태어난 너같은 얘들은 모두 이름말고는 기억이 나지 않지."+"\n"+"그리고 바깥에 나가자마자 생명력이 점점 깎이게 되어 죽고말아."
            , "그럼 어떻게 살아갈 수 있지?"+"\n"+"내게 무슨 일이 일어난건지 알아야만해."+"\n"+"날 이렇게 만든 누군가에게 복수할거야."+"\n"+"당장 이곳에서 나가야겠어."
            , "굳이 바깥으로 나가고 싶다면 몬스터들을 죽이고 생명을 뺏어."+"\n"+"그럼 너의 생명력을 유지할 수 있어."+"\n"+"하지만 쉽지 않을거다.. 괴물아, 너의 목숨을 소중히 여겨."
            , "다시 말하지만 난 괴물이 아니야."+"\n"+"알려준건 고맙다. 이만 갈게."
            , "그래.. 그럼 행운을 비마."});
        talkData.Add(20 + 1000, new string[] { "주변 몬스터들을 좀 죽여줄게.", "뭐래 날 위한 척 하지마." });
        talkData.Add(1000, new string[] { "안녕?", "잘 살아있구나 괴물아..", "난 괴물이 아니라니까?!" });

        //농부
        talkData.Add(10 + 2000, new string[] { "동굴에 있던 노숙자가 신경쓰여. 만나고 오자." });
        talkData.Add(11 + 2000, new string[] { "저기.."
            , "오! 오랜만에 보는 버려진 괴물이로군"
            , "뭐? 버려진 괴물이라니?"
            , "시내에 있는 Humanity라는 곳에서 이 섬까지 와서 폐기물들을 버린다고 들었네."+"\n"+"그 중에서 다양한 괴물들이 나타나곤 하지."+"\n"+"너같이 지성이 있는 얘들도 가끔씩 말이야."+"\n"+"그래서 이 섬에 몬스터들이 많아졌다네..천벌 받을것들!!"
            , "무슨 폐기물?"
            , "그거까진 모르겠구만.."+"\n"+"어찌됐든 그래서 주변에 몬스터들이 농사를 못짓게 자꾸 방해한단 말일세.."+"\n"+"자네가 20마리 정도 죽여주면 답례로 돈을 좀 주겠네. 도와주겠나?"});
        talkData.Add(21 + 2000, new string[] { "몬스터 20마리정도 잡아줘야 겠어." });
        talkData.Add(22 + 2000, new string[] { "다 잡았어.", "고맙구려. 여기 답례일세." });
        talkData.Add(2000, new string[] { "할아버지 몇살이야?", "비밀일세..." });

        //뱃사공
        talkData.Add(30 + 3000, new string[] { "그건 너의 배인가? 어디로 가는거지?"
            , "난 국가 외각으로 나가려고해.\n여긴 국가와 동떨어진 섬이거든. \n딱 보아하니 넌 여기서 살고 있지 않은 괴물이군."
            , "아니. 난 괴물이 아니야..\n하지만 여기서 나가려고하는데, 나도 동행해도 되겠나?"
            , "좋아, 하지만 공짜로는 안되지. \n여기서 나가기 위해선 항상 저 로봇의 허락을 받아야돼. \n하지만 솔직히 매일 허락을 받고 있는 입장에서 그건 좀 귀찮거든?\n너가 만약 이 앞에 있는 로봇을 없애준다면 \n내 배에 태워주지." });
        talkData.Add(31 + 3000, new string[] { "빨리 로봇을 없애고 이 섬을 빠져나가야 겠어." });
        talkData.Add(32 + 3000, new string[] { "죽이고 왔어.", "배에 타면 출발하지." });
        talkData.Add(3000, new string[] { "배 좀 태워주지?", "그런 식으로 나오면 안태워준다." });

        //주민들
        talkData.Add(40 + 4000, new string[] { "(주민들이 대화하고 있군.)","한 청년이 자신에 복종하는 복제품을 만들고 싶어했나봐. \n그래서 인체실험을 강행하고 조금만이라도 대들면 \n죽여서 폐기하는걸 반복한다 하더라고, \n잔인하지 않냐?"
            , "(복제품..? 그럼 내가 그 인간의 복제품에서 나온 폐기품인건가?)"
            , "말도 안돼.. 인간의 탈을 쓴 괴물아니야?"
            , "(그렇다면 난... 정말 괴물인가..? \n... \n그 인간을 죽이면 .. 내가 걔가 되는거잖아? \n맞아! 그 자식이 없으면 내가 인간이 될 수 있어!! \n없애야해.. 내가 죽여버리면 돼!! 그 인간을 찾아야겠어.)" 
            , "정말 끔찍하다니까."
            , "어이, 당신들 그 사람은 어디에 있지?"
            , "넌 뭐야, 저리안가?! 괴물주제에 끼어들고 있어."
            , "안되겠군.. 더 돌아다녀봐야겠어."});
        talkData.Add(4000, new string[] { "안녕.", "뭐야 이 괴물은, 저리안가?" });

        //아이
        talkData.Add(40 + 5000, new string[] { "주민들끼리 대화하고 있던데, 무슨 말을 하고 있는지 들어봐야겠어." });
        talkData.Add(41 + 5000, new string[] { "아이야. 왜 이런 곳에 있니.", "살려주세요!!! \n제 비밀기지에 왔다가 \n괴물들한테 둘러싸여 있어요..!ㅠㅠ","(무섭지 않게 주변 몬스터들을 없애줘야겠어.)"});
        talkData.Add(51 + 5000, new string[] { "(무섭지 않게 주변 몬스터들을 없애줘야겠어.)" });
        talkData.Add(52 + 5000, new string[] { "이제 좀 괜찮니?","감사합니다!! 아저씨는 혹시 저를 구하러온 배트맨인건가요?!","어..? 나는.. 괴물이야.","몸이 모두 검정색이라니 완전 멋져요! 제 영웅이에요!! \n정말 고마워요 영웅 아저씨!" 
            ,"그래."
            ,"멋있어..! \n이건 제가 가지고 있던 물약인데!\n아저씨가 가지세요!ㅎㅎ"
            ,"고마워.\n..\n(이런 순수한 아이한테는 내가 괴물인지 인간인지 중요하지 않구나. \n겉모습이 아닌 나를 봐주다니.. \n난 인간이 되고자했지만, 순수하지 않았구나.)"});
        talkData.Add(5000, new string[] { "여기는 뭐하는 곳이니?", "여기는 제 비밀기지에요!ㅎㅎ" });

        //못나가는 상인
        talkData.Add(41 + 6000, new string[] { "뒤쪽에 아이가 혼자 있어서 신경쓰여. 무슨 상황인지 보고 오자." });
        talkData.Add(60 + 6000, new string[] { "여기서 뭘하고 있지?"
            , "앞 돌산에 있는 괴물때문에\n시내로 나가는 차들이 못나가고 있어.\n언제쯤 갈 수 있을지.. 가족들을 만나야 하는데."
            , "그것만 없애만 Humanity로 데려다 줄 수 있나?"
            , "Humanity? 도시에 있는 복제품 회사말인가? \n저런 괴물만 없애준다면 당연하지! \n아는 사람이 거기에 있어서, 회사 안까지 데려다 줄 수 있다네.\n하지만 누가 저런 괴물을 죽일 수 있겠어?" });
        talkData.Add(61 + 6000, new string[] { "빨리 로봇을 없애고 이 섬을 빠져나가야 겠어." });
        talkData.Add(62 + 6000, new string[] { "죽이고 왔어.", "이런 고마워!!! 넌 우리의 영웅이야!!! \n차에 타거라! \n겉모습이 그러니, \n내가 잘 숨겨서 Humanity본거지까지 데려다주마." });
        talkData.Add(6000, new string[] { "바빠보이는군." });

        //연구실 안까지 도와준 상인
        talkData.Add(7000, new string[] { });
        talkData.Add(71 + 7000, new string[] { "데려다줘서 고마워.","그래. 너가 찾는 인간은 이 앞으로 들어가면 있단다.\n정말 그 괴물을 죽여줘서 고맙구나. \n너같은 착한 괴물이 있어 다행이야."
            , "난 괴물이 아니라 인간이야."
            , "얘야. 인간에 너무 집착하지 마렴\n괴물같은 짓을 하는 사람도 어쩔 수 없는 인간이야.\n괴물이라고 다 나쁜게 아니란다. \n너만 해도 우리를 도와주었지 않았니? \n넌 너만의 삶을 살았으면 좋겠다."
            , "그런가.. \n난 그 자식을 죽이면 정말 사람이 되는것인가..? \n내가 괴물이라고 해서 나쁜걸까?\n사람이란 무엇이지?" });

        //보스
        talkData.Add(8000, new string[] { });
        talkData.Add(71 + 8000, new string[] { "드디어 만났구나.","너는..! \n너가 어떻게 살아있는거지..?","난 너가 만든 112번째 복제품 R112이다. \n너의 그 갖잖은 욕심때문에 \n얼마나 많은 생명들이 죽어났는지 알고있어?"
            , "그래봤자. 넌 불량품일 뿐이야.\n여기까지 오느라 수고했고, \n내 훌륭한 실험체의 손에 이만 죽어라." });

        // 인간A를 죽이지 않는다. → Ending1. 나 자신으로써 살아간다.
        talkData.Add(81 + 8000, new string[] { "끝났군.","큭...괴물주제에..","맞아 난 괴물이야. 너가 만들어냈지.\n하지만 넌 인간도 괴물도 아니야.\n쓰레기 일뿐."
            ,"날 왜 죽이지 않는거지?","너에게 복수하러 왔을뿐이다.\n이제 난 내 삶을 살아야겠어.","너는 내 복제품일 뿐이야!!\n내가 없다면 넌 살 가치가 없다고!"
            ,"아니. 난 너가 아니야.\n그리고 이젠 너같은 인간이 되려고도 집착하려고도,\n괴물인 나를 욕하지도 않을거야.\n난 나 그대로 살아가겠어." });

        // 인간A를 죽이지 않는다. → Ending2. 진짜 인간A가 된다.
        talkData.Add(82 + 8000, new string[] { "나는 너 때문에 태어나서부터\n내 운명을 스스로 결정하지고 못했지.\n난 여기까지 오면서도 매일을..\n괴물 취급을 당하며 살아왔어..","큭.. 그래서 날 죽이겠다고?","그래. 나는 널 죽이고 진짜 인간A가 되겠어." });
    }

    private string GetTalk(int id, int talkIndex)
    {

        if (!talkData.ContainsKey(id))
        {
            if (id > 0)
            {
                if (!talkData.ContainsKey(id - id % 10))
                {
                    return GetTalk(id - id % 100, talkIndex);//퀘스트 맨 처음 대사가 없을때, 기본 대사를 가져오기
                }
                else
                {
                    return GetTalk(id - id % 10, talkIndex);//해당 퀘스트 진행 순서 대사가 없을 때, 퀘스트 맨 처음 대사 가져오기
                }
            }
        }
        if (talkIndex == talkData[id].Length)
            return null;
        else
        {
            if (talkIndex + 1 == talkData[id].Length)
            {
                // 마지막 문장일 경우
                // text "닫기"로 변경
                TMP_Text nextText = nextObject.GetComponent<TMP_Text>();
                nextText.text = "닫기 (F)";
            }
            else
            {
                // 마지막 문장이 아닐 경우
                // text "다음"으로 변경
                TMP_Text nextText = nextObject.GetComponent<TMP_Text>();
                nextText.text = "다음 (F)";
            }
            return talkData[id][talkIndex];
        }
    }
    private bool playerName = false;
    private void OnTalk(int id, string npcName)
    {

        //퀘스트 번호 가져오기
        int questIndex = IngameManager.instance.questManager.GetQuestIndex(id);
        int talkDataIndex = id + questIndex;
        string talkData = GetTalk(talkDataIndex, talkIndex);

        //대화 퀘스트 확인
        int[] TalkQuestIndex = { 1010, 2022, 3032, 4040, 5052, 6062, 8071 };
        if (Array.IndexOf(TalkQuestIndex, talkDataIndex) != -1)
        {
            if (talkData == null)
            {
                IngameManager.instance.questManager.TalkQuest(id);
                isConvo = false;
                playerName = false;
                talkIndex = 0;
                if (talkDataIndex == 8071) // 마지막 보스
                {
                    boss_Spawn.Spawn();
                }
                return;
            }
        }

        //몬스터 처치 퀘스트 확인
        int[] KillQuestIndex = { 2021, 5051 };
        if (Array.IndexOf(KillQuestIndex, talkDataIndex) != -1)
        {
            IngameManager.instance.questManager.MonsterQuest(id);

            questIndex = IngameManager.instance.questManager.GetQuestIndex(id);
            talkDataIndex = id + questIndex;
            talkData = GetTalk(talkDataIndex, talkIndex);
        }

        //보스 처치 퀘스트 확인
        int[] BossQuestIndex = { 3031, 6061, 8072 };
        if (Array.IndexOf(BossQuestIndex, talkDataIndex) != -1)
        {
            IngameManager.instance.questManager.BossQuest(id);

            questIndex = IngameManager.instance.questManager.GetQuestIndex(id);
            talkDataIndex = id + questIndex;
            talkData = GetTalk(talkDataIndex, talkIndex);
            if (talkDataIndex == 8072)
                boss_Spawn.Spawn();
        }

        //퀘스트 수락 창 생성하는 대화
        int[] QuestUIIndex = { 2011, 3030, 5041, 6060 };
        if (Array.IndexOf(QuestUIIndex, talkDataIndex) != -1)
        {
            if (talkData == null)
            {
                IngameManager.instance.uiManager.questUI.OpenQuestPanel(id);
                isConvo = false;
                playerName = false;
                talkIndex = 0;
                return;
            }
        }

        //엔딩
        if (talkDataIndex == 8081)
        {
            if (talkData == null)
            {
                isConvo = false;
                IngameManager.instance.questManager.questActionIndex += 3;
                IngameManager.instance.uiManager.endingUI.EndingCredit_1();
                return;
            }
        }
        if (talkDataIndex == 8082)
        {
            if (talkData == null)
            {
                isConvo = false;
                IngameManager.instance.questManager.questActionIndex += 3;
                IngameManager.instance.uiManager.endingUI.EndingCredit_2();
                return;
            }
        }


        //대화가 끝났을 경우
        if (talkData == null)
        {
            isConvo = false;
            playerName = false;
            talkIndex = 0;
            return;
        }
        isConvo = true;

        // 번갈아 가면서 NPC이름 변경
        TMP_Text npcText = nameObject.GetComponent<TMP_Text>();
        npcText.text = playerName ? npcName : string.Empty;
        playerName = !playerName;

        TMP_Text conversationText = conversationObject.GetComponent<TMP_Text>();
        conversationText.text = talkData;

        talkIndex++;
    }

    public void ShowText(GameObject scanObj)
    {
        if (scanObj == null)
        {
            isConvo = false;
            playerName = false;
            talkIndex = 0;
            talkObject.SetActive(isConvo);
            return;
        }

        scanObject = scanObj;
        NpcData npcData = scanObject.GetComponent<NpcData>();
        OnTalk(npcData.id, npcData.npcName);

        talkObject.SetActive(isConvo);

        // 대화중일 경우 움직임 제어
        /*
        if (isConvo)
            IngameManager.instance.playerController.OpenMenu();
        else
            IngameManager.instance.playerController.CloseMenu();
        */
    }
}