//using Photon.Pun;
//using UnityEngine;

//public class PlayerSetUp : MonoBehaviour
//{
//    public Camera PlayerCamera; // Camera1�� (������ Ŭ���̾�Ʈ��)
//    public Camera OpponentCamera; // Camera2�� (�ٸ� �÷��̾��)

//    void Start()
//    {
//        // �ʱ�ȭ: ��� ī�޶� ��Ȱ��ȭ
//        PlayerCamera.gameObject.SetActive(false);
//        OpponentCamera.gameObject.SetActive(false);

//        // ������ Ŭ���̾�Ʈ�� ��� Camera1 Ȱ��ȭ
//        if (PhotonNetwork.IsMasterClient)
//        {
//            PlayerCamera.gameObject.SetActive(true);
//        }
//        else // ������ �÷��̾�(���⼱ 2P)�� ��� Camera2 Ȱ��ȭ
//        {
//            OpponentCamera.gameObject.SetActive(true);
//        }
//    }
//}
