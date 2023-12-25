using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Exit : MonoBehaviourPun
{
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("LeaveRoomRPC", RpcTarget.All);
            }
            else
            {
                LeaveRoomProcess();
            }
        };
    }

    [PunRPC]
    private IEnumerator LeaveRoomRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트 본인 외 일반 클라이언트가 다 방에서 나갈때까지 대기
            while (PhotonNetwork.PlayerList.Length > 1)
            {
                yield return null;
            }
        }
        LeaveRoomProcess();
    }

    private void LeaveRoomProcess()
    {
        SystemManager.instance.dbManager.SaveInventory();
        IngameManager.instance.questManager.SyncQuestInfo();
        SystemManager.instance.dbManager.SaveStats();

        PhotonNetwork.LeaveRoom();
    }
}