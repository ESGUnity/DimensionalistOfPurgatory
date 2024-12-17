//using Photon.Pun;
//using Photon.Realtime;
//using UnityEditor.XR;
//using UnityEngine;
//using UnityEngine.UI;

//public class NetworkManager : MonoBehaviourPunCallbacks
//{

//    static NetworkManager instance;
//    public static NetworkManager Instance {  get { return instance; } }
//    private void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(gameObject);
//        }
//        else
//        {
//            instance = this;
//            DontDestroyOnLoad(gameObject);
//        }

//        PhotonNetwork.SendRate = 60; // 동기화 속도 향상을 위함
//        PhotonNetwork.SerializationRate = 30; // 동기화 속도 향상을 위함
//        PhotonNetwork.AutomaticallySyncScene = false; // 모든 플레이어의 자동 씬 동기화를 위함. true가 돼야하는데 일단 실험삼아 false로 함
//    }

//    //public override void OnConnectedToMaster()
//    //{
//    //    print("서버접속완료");
//    //    PhotonNetwork.LocalPlayer.NickName = MainMenuManager.Instance.NickNameInput.text;
//    //}
//    //public override void OnConnected()
//    //{
//    //    MainMenuManager.Instance.LoginPanel.SetActive(false);
//    //}
//    public override void OnJoinRandomFailed(short returnCode, string message) // 랜덤 룸 참가에 실패했을 때 호출
//    {
//        RoomOptions roomOptions = new RoomOptions();
//        roomOptions.MaxPlayers = 2;
//        PhotonNetwork.CreateRoom(null, roomOptions);
//    }
//    public override void OnJoinedRoom()
//    {
//        if (PhotonNetwork.IsMasterClient)
//        {
//            PhotonNetwork.LoadLevel("BattleScene");
//        }
//    }

//    // 버튼 함수
//    public void ConnectServer()
//    {
//        PhotonNetwork.ConnectUsingSettings();
//    }
//    public void DisconnectServer()
//    {
//        PhotonNetwork.Disconnect();
//    }
//    public void ConnectRoom()
//    {
//        PhotonNetwork.JoinRandomRoom();
//    }
//}
