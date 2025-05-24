using UnityEngine;
using Photon.Pun;
using GameManagers;
using Utility;

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
        photonView.RPC(method, RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SpawnWagon(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/Momo_Wagon1PF", position, rotation, 0);
        ChatManager.AddLine("Spawned a wagon.");
    }

    [PunRPC]
    public void DespawnWagon(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        GameObject wagon = FindNearestWagon();

        if (wagon == null || Vector3.Distance(transform.position, wagon.transform.position) > 20)
            return;

        Destroy(wagon);
        ChatManager.AddLine("Destroyed a wagon.");
    }

    [PunRPC]
    public void MountWagon(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        GameObject wagonObject = FindNearestWagon();
        Transform horse = FindMyHorse();

        if (Vector3.Distance(wagonObject.transform.position, horse.position) > 20) //mount range
            return;

        if (horse != null)
        {
            //set the connected body if the horse is a Rigidbody
            Rigidbody horseRigidbody = horse.GetComponent<Rigidbody>();
            if (horseRigidbody != null)
            {
                PhysicsWagon wagon = wagonObject.GetComponent<PhysicsWagon>();

                wagon.HorseHinge.transform.SetPositionAndRotation(horse.position - horse.transform.forward * 2.3f + Vector3.up * 0.6f, horse.gameObject.transform.rotation * Quaternion.Euler(90, 0, 0));
                wagon.HorseHinge.connectedBody = horseRigidbody;
                wagon.SetKinematic(false);
                wagon.isMounted = true;
                _mountedWagon = wagonObject;

                ChatManager.AddLine("Mounted the wagon.");
            }
        }
        else
        {
            Debug.LogWarning("Parent or Child is not assigned!");
        }
    }

    [PunRPC]
    public void UnmountWagon(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        if (_mountedWagon == null) {
            ChatManager.AddLine("You're not mounted on a wagon!", ChatTextColor.Error);
            return;
        }

        PhysicsWagon wagon = _mountedWagon.GetComponent<PhysicsWagon>();

        if (wagon == null)
            return;

        wagon.HorseHinge.connectedBody = wagon.TemporaryHinge;
        wagon.isMounted = false;
        wagon.SetKinematic(true);

        ChatManager.AddLine("Unmounted the wagon.");
    }

    [PunRPC]
    public void SpawnStation(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/SupplyStation", position, rotation, 0);
        ChatManager.AddLine("Spawned a station.");
    }

    [PunRPC]
    public void DespawnStation(PhotonMessageInfo Sender)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer")) {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        GameObject station = FindNearestStation();

        if (station == null || Vector3.Distance(transform.position, station.transform.position) > 20)
            return;

        Destroy(station);
        ChatManager.AddLine("Destroyed a station.");
    }

    public GameObject FindNearestWagon()
    {
        GameObject[] wagons = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID); // Get all objects in the scene
        GameObject nearestWagon = null;
        float nearestDistance = 1500f; //max range to look for wagons

        foreach (GameObject obj in wagons)
        {
            if (obj.name.Contains("Momo_Wagon")) // Check if the object is a "Wagon"
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWagon = obj;
                }
            }
        }

        if (nearestWagon != null)
            return nearestWagon;
        else
            return null;
    }

    public GameObject FindNearestStation()
    {
        GameObject[] wagons = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID); // Get all objects in the scene
        GameObject nearestWagon = null;
        float nearestDistance = 1500f; //max range to look for wagons

        foreach (GameObject obj in wagons)
        {
            if (obj.name.Contains("SupplyStation")) // Check if the object is a "Wagon"
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWagon = obj;
                }
            }
        }

        if (nearestWagon != null)
            return nearestWagon;
        else
            return null;
    }

    public Transform FindMyHorse()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(sortMode: FindObjectsSortMode.None);
        Transform myHorse = null;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Horse"))
            {
                PhotonView pv = obj.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    myHorse = obj.transform;
                    break;
                }
            }
        }

        if (myHorse != null) {
            return myHorse;
        } else {
            ChatManager.AddLine($"No horse found with my PhotonView!", ChatTextColor.Error);
            return null;
        }
    }

    public bool CheckIsMounted()
    {
        return _mountedWagon != null;
    }
}
