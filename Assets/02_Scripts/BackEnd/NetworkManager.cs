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

//        PhotonNetwork.SendRate = 60; // ����ȭ �ӵ� ����� ����
//        PhotonNetwork.SerializationRate = 30; // ����ȭ �ӵ� ����� ����
//        PhotonNetwork.AutomaticallySyncScene = false; // ��� �÷��̾��� �ڵ� �� ����ȭ�� ����. true�� �ž��ϴµ� �ϴ� ������ false�� ��
//    }

//    //public override void OnConnectedToMaster()
//    //{
//    //    print("�������ӿϷ�");
//    //    PhotonNetwork.LocalPlayer.NickName = MainMenuManager.Instance.NickNameInput.text;
//    //}
//    //public override void OnConnected()
//    //{
//    //    MainMenuManager.Instance.LoginPanel.SetActive(false);
//    //}
//    public override void OnJoinRandomFailed(short returnCode, string message) // ���� �� ������ �������� �� ȣ��
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

//    // ��ư �Լ�
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
