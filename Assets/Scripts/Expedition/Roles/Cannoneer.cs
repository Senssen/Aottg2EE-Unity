using UnityEngine;
using Photon.Pun;

public class Cannoneer : MonoBehaviour
{
    private GameObject Cannon = null;
    public void SpawnCannonController()
    {
        if (Cannon == null) {
            Vector3 pos = transform.position + (transform.forward * 1f);
            Cannon = PhotonNetwork.Instantiate("Map/Interact/Prefabs/CannoneerCannon", pos, transform.rotation);
        } else {
            Cannon.GetComponent<CannoneerCannon>().UnMount();
        }
    }
}