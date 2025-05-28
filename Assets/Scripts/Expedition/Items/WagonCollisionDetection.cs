using UnityEngine;
using Photon.Pun;

public class WagonCollisionDetection : MonoBehaviour
{
    [SerializeField] private GameObject wagon;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Contains("titan")) {
            Wagoneer wagoneer = PhotonExtensions.GetMyPlayer().GetComponent<Wagoneer>();
            if (wagoneer) {
                wagoneer.gameObject.GetPhotonView().RPC("ShowWagonTextRPC", RpcTarget.All);
            }

            PhotonNetwork.Destroy(wagon);
        }
    }

}
