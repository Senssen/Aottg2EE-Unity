using Characters;
using GameManagers;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class Wagoneer : MonoBehaviour
{
    private GameObject _mountedWagon;
    private PhotonView photonView;
    [SerializeField] private float SPEED_OVERRIDE = 15f;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SendRPC(string method)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        photonView.RPC(method, RpcTarget.AllBuffered, photonView.ViewID);
    }

    public void SpawnWagon() // the view ID does not matter when spawning the wagon
    {
        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        GameObject wagon = FindNearestObjectByName("Momo_Wagon");
        if (wagon == null || Vector3.Distance(wagon.transform.position, position) > 10f) {
            PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/Momo_Wagon1PF", position, rotation, 0);
            ChatManager.AddLine("Spawned a wagon.");
        } else {
            ChatManager.AddLine("A wagon already exists in close proximity.");
        }
    }

    public void DespawnWagon()
    {
        GameObject wagon = FindNearestObjectByName("Momo_Wagon");

        if (wagon != null && wagon.TryGetComponent(out PhysicsWagon _physicsWagon) && _physicsWagon.GetDistance(transform) < 20f)
        {
            FixedJoint joint = _physicsWagon.GetJoint();
            if (joint && joint.connectedBody.TryGetComponent(out Horse horse))
                horse.SetSpeedOverride(1f);

            PhotonNetwork.Destroy(wagon);
            ChatManager.AddLine("Destroyed a wagon.");
        }
    }

    [PunRPC]
    public void MountWagon(int wagoneerViewId, PhotonMessageInfo Sender)
    {
        if (PhotonNetwork.GetPhotonView(wagoneerViewId).TryGetComponent(out Wagoneer wagoneer) && wagoneer.CheckIsMounted() == true) {
            if (Sender.photonView.IsMine)
                ChatManager.AddLine("You are already mounting a wagon!");
            return;
        }

        GameObject wagonObject = FindNearestObjectByName(wagoneerViewId, "Momo_Wagon");
        Transform horse = FindHorseOfViewId(wagoneerViewId);

        if (horse != null && horse.TryGetComponent(out Horse horseScript) && wagonObject != null && wagonObject.TryGetComponent(out PhysicsWagon wagon) && wagon.GetDistance(transform) < 20)
        {
            Rigidbody horseRigidbody = horse.GetComponent<Rigidbody>();
            if (horseRigidbody != null)
            {
                if (wagon.GetIsMounted()) {
                    if (Sender.photonView.IsMine)
                        ChatManager.AddLine("The wagon is already mounted by another wagoneer!");

                    return;
                }

                wagon.transform.SetPositionAndRotation(horseScript.WagonHarness.position, horseScript.WagonHarness.rotation);

                wagon.CreateJoint(horseRigidbody);
                horseScript.SetSpeedOverride(SPEED_OVERRIDE);

                wagon.SetIsMounted(true);
                _mountedWagon = wagonObject;

                if (Sender.photonView.IsMine)
                    ChatManager.AddLine("Mounted the wagon.");
            }
        }
        else
        {
            Debug.LogWarning("Parent or Child is not assigned!");
        }
    }

    [PunRPC]
    public void UnmountWagon(int wagoneerViewId, PhotonMessageInfo Sender)
    {
        if (PhotonNetwork.GetPhotonView(wagoneerViewId).TryGetComponent(out Wagoneer wagoneer)) {
            if (wagoneer._mountedWagon == null && Sender.photonView.IsMine) {
                ChatManager.AddLine("You're not mounted on a wagon!", ChatTextColor.Error);
                return;
            }

        if (wagoneer._mountedWagon.TryGetComponent(out PhysicsWagon wagon) && wagon.GetJoint().connectedBody.TryGetComponent(out Horse horse)) {
                wagon.Beams.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                horse.SetSpeedOverride(1f);
                wagon.DestroyJoint();
                wagon.SetIsMounted(false);
                wagoneer._mountedWagon = null;

                if (Sender.photonView.IsMine)
                    ChatManager.AddLine("Unmounted the wagon.");
            }
        }
    }

    public void SpawnStation()
    {
        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/SupplyStation", position, rotation, 0);
        ChatManager.AddLine("Spawned a station.");
    }

    public void DespawnStation()
    {
        GameObject station = FindNearestObjectByName("SupplyStation");
        if (station == null || Vector3.Distance(transform.position, station.transform.position) > 20)
            return;

        PhotonNetwork.Destroy(station);
        ChatManager.AddLine("Destroyed a station.");
    }

    public GameObject FindNearestObjectByName(int senderViewId, string name)
    {
        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearestObject = null;
        float nearestDistance = 1500f;

        Transform senderTransform = PhotonNetwork.GetPhotonView(senderViewId).GetComponent<Transform>();
        if (senderTransform == null)
            return null;

        foreach (GameObject go in objects)
        {
            if (go.name.Contains(name))
            {
                float distance = Vector3.Distance(senderTransform.position, go.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = go;
                }
            }
        }

        if (nearestObject != null)
            return nearestObject;
        else
            return null;
    }

    public GameObject FindNearestObjectByName(string name)
    {
        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearestObject = null;
        float nearestDistance = 1500f;
        foreach (GameObject go in objects)
        {
            if (go.name.Contains(name))
            {
                float distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = go;
                }
            }
        }

        if (nearestObject != null)
            return nearestObject;
        else
            return null;
    }

    public Transform FindHorseOfViewId(int senderViewId)
    {
        Horse[] horses = FindObjectsByType<Horse>(FindObjectsSortMode.None);
        for (int idx = 0; idx < horses.Length; idx++) {
            if (horses[idx].photonView.OwnerActorNr == PhotonNetwork.GetPhotonView(senderViewId).OwnerActorNr) {
                return horses[idx].Cache.Transform;
            }
        }

        return null;
    }

    public bool CheckIsMounted()
    {
        return _mountedWagon != null;
    }

    // this is shown to all players on wagon destroy. Since the wagon is destroyed, putting this method here makes more sense.

    [PunRPC]
    public void ShowWagonTextRPC(PhotonMessageInfo sender)
    {
        GameObject.Find("Expedition UI(Clone)").GetComponent<WagoneerMenuManager>().ShowWagonText();
    }
}
