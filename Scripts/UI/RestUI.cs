using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestUI : MonoBehaviour
{
    [SerializeField] private GameObject restButton;
    [SerializeField] private GameObject restPanel;
    [SerializeField] private GameObject savepointPanel;

    private void Start()
    {
        restPanel.SetActive(false);
        savepointPanel.SetActive(false);
    }

    public void ToggleRestButton()
    {
        restButton.SetActive(!restButton.activeSelf);
    }

    public void OpenMenu()
    {
        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        IngameManager.instance.uiManager.fadeInOut.StartFadeOutEffect();
        IngameManager.instance.playerController.OpenMenu(); // 캐릭터 움직임 멈춤

        //메뉴창 오픈
        yield return new WaitForSeconds(2.0f);
        restPanel.SetActive(true);
        restButton.SetActive(false);

        SystemManager.instance.dbManager.SaveInventory();
        IngameManager.instance.questManager.SyncQuestInfo();
        SystemManager.instance.dbManager.SaveStats();

        IngameManager.instance.uiManager.fadeInOut.StartFadeInEffect();
    }

    public void CloseMenu()
    {
        restPanel.SetActive(false);
        restButton.SetActive(true);

        IngameManager.instance.playerController.CloseMenu(); // 캐릭터 움직임 다시 시작
    }

    public void OpenSavepoint()
    {
        savepointPanel.SetActive(true);
        restPanel.SetActive(false);
    }

    public void CloseSavepoint()
    {
        savepointPanel.SetActive(false);
        restPanel.SetActive(true);
    }
}
