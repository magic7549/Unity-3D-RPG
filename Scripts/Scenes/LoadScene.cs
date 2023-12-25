using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoadScene : MonoBehaviourPun
{
    public void LoadSceneProcess(string sceneName)
    {
        SystemManager.instance.dbManager.SaveInventory();
        IngameManager.instance.questManager.SyncQuestInfo();
        SystemManager.instance.dbManager.SaveStats();

        if (PhotonNetwork.IsMasterClient)
            LoadSceneRPC(sceneName);
        else
            photonView.RPC("LoadSceneRPC", RpcTarget.MasterClient, sceneName);
    }

    [PunRPC]
    private void LoadSceneRPC(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
