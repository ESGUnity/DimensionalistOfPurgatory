//using Photon.Pun;
//using UnityEngine;

//public class PlayerSetUp : MonoBehaviour
//{
//    public Camera PlayerCamera; // Camera1번 (마스터 클라이언트용)
//    public Camera OpponentCamera; // Camera2번 (다른 플레이어용)

//    void Start()
//    {
//        // 초기화: 모든 카메라 비활성화
//        PlayerCamera.gameObject.SetActive(false);
//        OpponentCamera.gameObject.SetActive(false);

//        // 마스터 클라이언트인 경우 Camera1 활성화
//        if (PhotonNetwork.IsMasterClient)
//        {
//            PlayerCamera.gameObject.SetActive(true);
//        }
//        else // 나머지 플레이어(여기선 2P)인 경우 Camera2 활성화
//        {
//            OpponentCamera.gameObject.SetActive(true);
//        }
//    }
//}
