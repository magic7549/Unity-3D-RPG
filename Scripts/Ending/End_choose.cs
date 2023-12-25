using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class End_choose : MonoBehaviour
{
    // Ending 1 버튼 클릭
    public void OnClickEnding1()
    {
        gameObject.SetActive(false);
        IngameManager.instance.questManager.questActionIndex += 1;
        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }

    // Ending 2 버튼 클릭
    public void OnClickEnding2()
    {
        gameObject.SetActive(false);
        IngameManager.instance.questManager.questActionIndex += 2;
        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }

}
