using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class IngameManager : MonoBehaviour
{
    public static IngameManager instance;

    private void Awake()
    {
        //게임 매니저 싱글톤 적용
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public UIManager uiManager;
    public GameObject playerPrefab;
    public PlayerController playerController;
    public Inventory inventory;
    public Equipment equipment;
    public PlayerWeapon weapon;
    public PlayerStat stat;
    public CameraMovement cameraMovement;
    public ItemDatabase itemDatabase;
    public QuestManager questManager;
    public LoadScene loadScene;

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        GameObject playerObject = null;

        string slot_str = "slot" + SystemManager.instance.selectSlotNum;
        for (int i = 0; i < SystemManager.instance.dbManager.userTable.Rows.Count; i++)
        {
            if (SystemManager.instance.dbManager.userTable.Rows[i]["slot"].ToString().Equals(slot_str))
            {
                // 다른 유저가 세이브포인트로 다른 씬으로 이동했을때 해당 맵에 처음부분으로 이동하기 위한 부분
                int num = (int)SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"];
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

                if (!SceneManager.GetActiveScene().name.Equals(map))
                {
                    if (SceneManager.GetActiveScene().name.Equals("Grassland"))
                        num = 0;
                    else if (SceneManager.GetActiveScene().name.Equals("Badlands"))
                        num = 3;
                    else
                        num = 8;
                }

                playerObject = PhotonNetwork.Instantiate(playerPrefab.name, uiManager.savepointUI.savepointVectors[num], Quaternion.identity);
            }
        }

        playerController = playerObject.GetComponent<PlayerController>();
        stat = playerObject.GetComponent<PlayerStat>();
        inventory = playerObject.GetComponent<Inventory>();
        equipment = playerObject.GetComponent<Equipment>();
        weapon = playerObject.GetComponent<PlayerWeapon>();
        loadScene = playerObject.GetComponent<LoadScene>();
    }
}
