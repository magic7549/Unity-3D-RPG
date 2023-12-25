using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeftRoom : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");

        Debug.Log("방 나가기");
    }
}
