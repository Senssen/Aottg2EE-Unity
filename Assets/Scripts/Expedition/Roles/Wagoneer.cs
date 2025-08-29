using UnityEngine;
using Photon.Pun;
using GameManagers;
using Utility;
using Characters;

public class Wagoneer : MonoBehaviour
{
    private GameObject _mountedWagon;
    private PhotonView photonView;

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

    [PunRPC]
    public void SpawnWagon(int wagoneerViewId, PhotonMessageInfo Sender) // the view ID does not matter when spawning the wagon
    {
        if (!Sender.photonView.IsMine)
            return;

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        GameObject wagon = FindNearestObjectByName(wagoneerViewId, "Momo_Wagon");
        if (wagon == null || Vector3.Distance(wagon.transform.position, position) > 10f) {
            PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/Momo_Wagon1PF", position, rotation, 0);
            if (Sender.photonView.IsMine)
                ChatManager.AddLine("Spawned a wagon.");
        } else if (Sender.photonView.IsMine) {
            ChatManager.AddLine("A wagon already exists in close proximity.");
        }
    }

    [PunRPC]
    public void DespawnWagon(int wagoneerViewId, PhotonMessageInfo Sender)
    {
        if (!Sender.photonView.IsMine)
            return;

        GameObject wagon = FindNearestObjectByName(wagoneerViewId, "Momo_Wagon");

        if (wagon != null)
        {
            if (wagon.TryGetComponent(out PhysicsWagon _physicsWagon) && _physicsWagon.GetDistance(transform) < 20f)
            {
                PhotonNetwork.Destroy(wagon);
                if (Sender.photonView.IsMine)
                    ChatManager.AddLine("Destroyed a wagon.");       
            }
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

        if (Vector3.Distance(wagonObject.transform.position, horse.position) > 20 || wagonObject == null || horse == null) //mount range
            return;

        if (horse != null)
        {
            Rigidbody horseRigidbody = horse.GetComponent<Rigidbody>();
            if (horseRigidbody != null)
            {
                PhysicsWagon wagon = wagonObject.GetComponent<PhysicsWagon>();
                if (wagon.isMounted) {
                    if (Sender.photonView.IsMine)
                        ChatManager.AddLine("The wagon is already mounted by another wagoneer!");

                    return;
                }

                wagon.HorseHinge.transform.SetPositionAndRotation(horse.position - horse.transform.forward * 2.3f + Vector3.up * 0.6f, horse.gameObject.transform.rotation * Quaternion.Euler(90, 0, 0));
                wagon.HorseHinge.connectedBody = horseRigidbody;
                wagon.isMounted = true;
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

            if (wagoneer._mountedWagon.TryGetComponent(out PhysicsWagon wagon)) {
                wagon.HorseHinge.connectedBody = wagon.TemporaryHinge;
                wagon.isMounted = false;
                wagoneer._mountedWagon = null;

                if (Sender.photonView.IsMine)
                    ChatManager.AddLine("Unmounted the wagon.");
            }
        }
    }

    [PunRPC]
    public void SpawnStation(int wagoneerViewId, PhotonMessageInfo Sender)
    {
        if (!Sender.photonView.IsMine)
            return;

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/SupplyStation", position, rotation, 0);

        if (Sender.photonView.IsMine)
            ChatManager.AddLine("Spawned a station.");
    }

    [PunRPC]
    public void DespawnStation(int wagoneerViewId, PhotonMessageInfo Sender)
    {
        if (!Sender.photonView.IsMine)
            return;

        GameObject station = FindNearestObjectByName(wagoneerViewId, "SupplyStation");

        if (station == null || Vector3.Distance(transform.position, station.transform.position) > 20)
            return;

        PhotonNetwork.Destroy(station);

        if (Sender.photonView.IsMine)
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
