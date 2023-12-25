using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TempLogin : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonConnect();
    }

    private void PhotonConnect()
    {
        // true일 경우 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = "1";

        // 서버 접속
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.NickName = "TTTTTT";

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("01. 포톤 서버에 접속");

        //로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.CreateRoom("room_1");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");
    }


    private void OnTriggerEnter(Collider other)
    {if (other.tag == "FlameDragonAttack")
        {
            Debug.Log((int)other.GetComponentInParent<Boss_FlameDragon>().realDmg);
        }
    }
}
