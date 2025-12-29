using Characters;
using GameManagers;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class Wagoneer : MonoBehaviour
{
    private GameObject _mountedWagon;
    private Human human;
    private PhotonView photonView;
    [SerializeField] private float MAX_SPEED_OVERRIDE = 15f;

    void Start()
    {
        human = GetComponent<Human>();
        photonView = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        if (human.Horse)
        {
            bool isRidingWagon = _mountedWagon != null && human.MountState == HumanMountState.Horse;
            float currentSpeedOverride = human.Horse.GetSpeedOverride();
            if (isRidingWagon && human.TargetMagnitude != 0 && currentSpeedOverride < MAX_SPEED_OVERRIDE)
            {
                float s = currentSpeedOverride + (4f * Time.fixedDeltaTime);
                human.Horse.SetSpeedOverride(s);
            }
            else if (isRidingWagon && human.TargetMagnitude == 0 && currentSpeedOverride > 1f && human.GetVelocity().magnitude > 0.25f)
            {
                float s = currentSpeedOverride - (2f * Time.fixedDeltaTime);
                human.Horse.SetSpeedOverride(s);
            }
            else if (isRidingWagon && human.TargetMagnitude == 0 && currentSpeedOverride > 1f && human.GetVelocity().magnitude <= 0.25f)
            {
                human.Horse.SetSpeedOverride(1f);
            }
        }
    }

    public void SendRPC(string method)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Wagoneer"))
        {
            ChatManager.AddLine("You must be a wagoneer to use this option!", ChatTextColor.Error);
            return;
        }

        photonView.RPC(method, RpcTarget.AllBuffered, photonView.ViewID);
    }

    public void SpawnWagon() // the view ID does not matter when spawning the wagon
    {
        if (photonView.Owner.CustomProperties.ContainsKey("Wagoneer") == false)
        {
            ChatManager.AddLine("Only a wagoneer can spawn a wagon.", ChatTextColor.Error);
            return;
        }

        Human human = GetComponent<Human>();
        if (human.State != HumanState.Idle)
        {
            ChatManager.AddLine("A wagon can't be spawned when not idle.", ChatTextColor.Error);
            return;
        }

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        GameObject wagon = FindNearestObjectByName("Momo_Wagon");
        if (wagon == null || Vector3.Distance(wagon.transform.position, position) > 10f)
        {
            PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/Momo_Wagon1PF", position, rotation, 0);
            ChatManager.AddLine("Spawned a wagon.");
        }
        else
        {
            ChatManager.AddLine("A wagon already exists in close proximity.", ChatTextColor.Error);
        }
    }

    public void DespawnWagon()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("Wagoneer") == false)
        {
            ChatManager.AddLine("Only a wagoneer can despawn a wagon.", ChatTextColor.Error);
            return;
        }

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
        PhotonView pv = PhotonNetwork.GetPhotonView(wagoneerViewId);
        if (pv.Owner.CustomProperties.ContainsKey("Wagoneer") == false)
        {
            if (pv.IsMine)
                ChatManager.AddLine("Only a wagoneer can mount a wagon.", ChatTextColor.Error);

            return;
        }

        if (pv.TryGetComponent(out Wagoneer wagoneer) && wagoneer.CheckIsMounted() == true)
        {
            if (Sender.photonView.IsMine)
                ChatManager.AddLine("You are already mounting a wagon!", ChatTextColor.Error);
            return;
        }

        GameObject wagonObject = FindNearestObjectByName(wagoneerViewId, "Momo_Wagon");
        Transform horse = FindHorseOfViewId(wagoneerViewId);

        if (horse != null && horse.TryGetComponent(out Horse horseScript) && wagonObject != null && wagonObject.TryGetComponent(out PhysicsWagon wagon))
        {
            if (wagon.GetDistance(horse.transform) > 2f)
            {
                ChatManager.AddLine("You need to be closer to the center of the wagon beams to mount the wagon.", ChatTextColor.Error);
                return;
            }

            Rigidbody horseRigidbody = horse.GetComponent<Rigidbody>();
            if (horseRigidbody != null)
            {
                if (wagon.GetIsMounted())
                {
                    if (Sender.photonView.IsMine)
                        ChatManager.AddLine("The wagon is already mounted by another wagoneer!", ChatTextColor.Error);

                    return;
                }

                horse.transform.SetPositionAndRotation(wagon.HorseSpot.position, wagon.HorseSpot.rotation);

                wagon.CreateJoint(horseRigidbody);
                horseScript.SetSpeedOverride(MAX_SPEED_OVERRIDE);

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
        if (PhotonNetwork.GetPhotonView(wagoneerViewId).TryGetComponent(out Wagoneer wagoneer))
        {
            if (wagoneer._mountedWagon == null && Sender.photonView.IsMine)
            {
                ChatManager.AddLine("You're not mounted on a wagon!", ChatTextColor.Error);
                return;
            }

            if (wagoneer._mountedWagon.TryGetComponent(out PhysicsWagon wagon) && wagon.GetJoint().connectedBody.TryGetComponent(out Horse horse))
            {
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
        if (photonView.Owner.CustomProperties.ContainsKey("Wagoneer") == false)
        {
            ChatManager.AddLine("Only a wagoneer can spawn a supply station.", ChatTextColor.Error);
            return;
        }

        Human human = GetComponent<Human>();
        if (human.MountState != HumanMountState.None || human.State != HumanState.Idle)
        {
            ChatManager.AddLine("A supply station can't be spawned when not idle on the ground.", ChatTextColor.Error);
            return;
        }

        Vector3 position = transform.position + transform.forward * 12f;
        Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        PhotonNetwork.Instantiate(ResourcePaths.Wagoneer + "/SupplyStation", position, rotation, 0);
        ChatManager.AddLine("Spawned a station.");
    }

    public void DespawnStation()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("Wagoneer") == false)
        {
            ChatManager.AddLine("Only a wagoneer can despawn a supply station.", ChatTextColor.Error);
            return;
        }

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
        for (int idx = 0; idx < horses.Length; idx++)
        {
            if (horses[idx].photonView.OwnerActorNr == PhotonNetwork.GetPhotonView(senderViewId).OwnerActorNr)
            {
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
    
    public void OnDeath()
    {
        GameObject.Find("Expedition UI(Clone)").GetComponent<WagoneerMenuManager>().SetSupplyStationText(false);
        if (_mountedWagon != null && _mountedWagon.TryGetComponent(out PhysicsWagon physicsWagon))
        {
            physicsWagon.SetIsMounted(false);
        }
    }
}
