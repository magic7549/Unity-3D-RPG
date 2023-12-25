using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject endingChoose;
    [SerializeField] private GameObject ending1;
    [SerializeField] private GameObject ending2;
    [SerializeField] private GameObject background;
    public EndingPanel endingpanel;


    public void OpenEndingChoose()
    {
        endingChoose.SetActive(true);
        IngameManager.instance.playerController.OpenMenu(); // 캐릭터 움직임 멈춤
    }

    // Ending 1 
    public void EndingCredit_1()
    {
        IngameManager.instance.playerController.OpenMenu(); // 캐릭터 움직임 멈춤
        background.SetActive(true);
        ending1.SetActive(true);
        StartCoroutine(EndingCoroutine()); 
    }

    // Ending 2 
    public void EndingCredit_2()
    {
        IngameManager.instance.playerController.OpenMenu(); // 캐릭터 움직임 멈춤
        background.SetActive(true);
        ending2.SetActive(true);
        StartCoroutine(EndingCoroutine());
    }

    // 30초간 엔딩을 위해 멈추기
    private IEnumerator EndingCoroutine()
    {
        endingpanel.StartEndingOutEffect();
        yield return new WaitForSeconds(35f);
        IngameManager.instance.uiManager.savepointUI.ClickTeleport(0);
        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }
}
