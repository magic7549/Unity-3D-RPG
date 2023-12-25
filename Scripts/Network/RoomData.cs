using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private TMP_Text RoomInfoText;
    private RoomInfo _roomInfo;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            // EX : room_03 (1/2)
            RoomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            // 버튼의 클릭 이벤트에 함수를 연결
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    void Awake()
    {
        RoomInfoText = GetComponentInChildren<TMP_Text>();
    }

    void OnEnterRoom(string roomName)
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.JoinRoom(roomName);
    }
}