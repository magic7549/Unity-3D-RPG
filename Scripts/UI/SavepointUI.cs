using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavepointUI : MonoBehaviour
{
    public GameObject[] savepointButtons;
    public FadeInOut fadeInOut;
    public RestUI restUI;
    public Vector3[] savepointVectors;

    private void OnEnable()
    {
        // 초기화
        // 모든 세이브 포인트 버튼 비활성화
        foreach (var item in savepointButtons)
        {
            item.SetActive(false);
        }

        // 한 번 도착한 세이브포인트 활성화
        for (int i = 0; i < savepointButtons.Length; i++)
        {
            if (IngameManager.instance.stat.savepoint_unlock[i].Equals("1"))
                savepointButtons[i].SetActive(true);
        }
    }

    public void ClickTeleport(int num)
    {
        string map;
        if (num < 3)
        {
            map = "Grassland";
        }
        else if (num < 8)
        {
            map = "Badlands";
        }
        else
        {
            map = "Laboratory";
        }

        // 현재맵에 속한 세이브포인트
        if (SceneManager.GetActiveScene().name.Equals(map))
        {
            IngameManager.instance.stat.lastUseSavepoint = num;
            StartCoroutine(TeleportProcess(num));
        }
        // 다른맵에 속한 세이브포인트
        else
        {
            string slot_str = "slot" + SystemManager.instance.selectSlotNum;
            for (int i = 0; i < SystemManager.instance.dbManager.userTable.Rows.Count; i++)
            {
                if (SystemManager.instance.dbManager.userTable.Rows[i]["slot"].ToString().Equals(slot_str))
                {
                    object unboxedValue = SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"];
                    int temp = (int)unboxedValue;
                    temp = num;
                    SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"] = temp;
                }
            }

            IngameManager.instance.stat.lastUseSavepoint = num;
            IngameManager.instance.loadScene.LoadSceneProcess(map);
        }

    }

    private IEnumerator TeleportProcess(int num)
    {
        fadeInOut.StartFadeOutEffect();

        yield return new WaitForSeconds(1.5f);
        IngameManager.instance.playerController.Teleport(savepointVectors[num]);

        yield return new WaitForSeconds(0.5f);
        fadeInOut.StartFadeInEffect();
        restUI.CloseSavepoint();
        restUI.CloseMenu();
    }

    
}
