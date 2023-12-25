using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1.2";

    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField roomMaxInput;

    [SerializeField] private GameObject waitingPanel;

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomMaxText;

    // 룸 목록 저장하기 위한 딕셔너리 자료형
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // 룸을 표시할 프리팹
    [SerializeField] private GameObject roomPrefab;
    // Room 프리팹이 차일드화 시킬 부모 객체
    [SerializeField] private Transform roomScrollContent;

    // 유저 닉네임을 표시할 프리팹
    [SerializeField] private GameObject userTextPrefab;
    // 유저 프리팹이 차일드화 시킬 부모 객체
    [SerializeField] private Transform userScrollContent;
    // 현재 표시 중인 유저명 프리팹 목록
    private List<GameObject> userPrefabList = new List<GameObject>();

    // 게임 시작 버튼
    [SerializeField] private GameObject startButton;

    private void Awake()
    {
        SystemManager.instance.dbManager.LoadCharacterDB();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // true일 경우 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        // 서버 접속
        if (!PhotonNetwork.IsConnected) 
            PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        Debug.Log("00. 포톤 매니저 시작");
        PhotonNetwork.NickName = SystemManager.instance.dbManager.user_id;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("01. 포톤 서버에 접속");

        //로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("02. 로비에 접속");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 룸 접속 실패");

        // 룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 2;

        roomNameInput.text = $"Room_{Random.Range(1, 100):000}";

        // 룸을 생성 > 자동 입장됨
        PhotonNetwork.CreateRoom("room_1", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04. 방 입장 완료");

        waitingPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomMaxText.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

        if (!PhotonNetwork.IsMasterClient)
            startButton.GetComponent<Button>().interactable = false;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempUser = Instantiate(userTextPrefab, userScrollContent);
            tempUser.GetComponent<TextMeshProUGUI>().text = player.NickName;
            userPrefabList.Add(tempUser);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string slot_str = "slot" + SystemManager.instance.selectSlotNum;
            for (int i = 0; i < SystemManager.instance.dbManager.userTable.Rows.Count; i++)
            {
                if (SystemManager.instance.dbManager.userTable.Rows[i]["slot"].ToString().Equals(slot_str))
                {
                    if ((int)SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"] < 3)
                    {
                        // 초원
                        PhotonNetwork.LoadLevel("Grassland");
                    }
                    else if ((int)SystemManager.instance.dbManager.userTable.Rows[i]["lastSavepoint"] < 8)
                    {
                        // 황무지
                        PhotonNetwork.LoadLevel("Badlands");
                    }
                    else
                    {
                        // 연구실
                        PhotonNetwork.LoadLevel("Laboratory");
                    }
                }
            }


            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void LeaveRoom()
    {
        TransferMasterClient();
        PhotonNetwork.LeaveRoom();
        for (int i = userPrefabList.Count - 1; i >= 0; i--)
        {
            GameObject userObject = userPrefabList[i];
            userPrefabList.RemoveAt(i);
            Destroy(userObject);
        }
    }

    // 방장 넘기기
    public void TransferMasterClient()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 현재 방에 있는 모든 플레이어 정보 가져오기
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length < 2)
        {
            // 방에 플레이어가 1명 이하인 경우, 방장을 그대로 유지
            return;
        }

        // 방장을 다른 플레이어에게 넘김
        Player newMasterClient = null;

        foreach (Player player in players)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                newMasterClient = player;
                break;
            }
        }

        if (newMasterClient != null)
        {
            PhotonNetwork.SetMasterClient(newMasterClient);
        }
    }

    // 방장이 바뀌었을 때 호출되는 이벤트
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("New Master Client: " + newMasterClient.NickName);

        // 게임시작 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
            startButton.GetComponent<Button>().interactable = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"플레이어 {newPlayer.NickName} 방 참가.");

        GameObject tempUser = Instantiate(userTextPrefab, userScrollContent);
        tempUser.GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;
        userPrefabList.Add(tempUser);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 {otherPlayer.NickName} 방 나감.");

        foreach (GameObject userObject in userPrefabList)
        {
            if (userObject.GetComponent<TextMeshProUGUI>().text == otherPlayer.NickName)
            {
                userPrefabList.Remove(userObject);
                Destroy(userObject);
                break;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            // 룸이 삭제된 경우
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            // 룸 정보가 갱신(변경)된 경우
            else
            {
                // 룸이 처음 생성된 경우
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, roomScrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                // 룸 정보를 갱신하는 경우
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }


    #region UI_BUTTON_CALLBACK
    // Room 생성 버튼 클릭 시
    public void OnMakeRoomClick()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = int.Parse(roomMaxInput.text);

        // 방 이름이 비어있으면
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            // 랜덤 룸 이름 부여
            roomNameInput.text = $"ROOM_{Random.Range(1, 100):000}";
        }

        PhotonNetwork.CreateRoom(roomNameInput.text, ro);
    }
    #endregion
}
