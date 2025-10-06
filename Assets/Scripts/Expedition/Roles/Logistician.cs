using UnityEngine;
using Photon.Pun;
using Utility;
using Settings;
using Cameras;
using ApplicationManagers;
using GameManagers;

public class Logistician : MonoBehaviour
{
    public int WeaponSupply;
    public int GasSupply;
    private LogisticianUiManager uiManager;
    [SerializeField]
    private GameObject SupplyPack;
    [SerializeField]
    public AudioSource GasCollectSound;
    [SerializeField]
    public AudioSource BladeCollectSound;
    [SerializeField]
    public AudioSource AmmoCollectSound;

    public void Start()
    {
        WeaponSupply = EmVariables.LogisticianMaxSupply;
        GasSupply = EmVariables.LogisticianMaxSupply;
        uiManager = GameObject.Find("Expedition UI(Clone)").GetComponent<LogisticianUiManager>();

        uiManager.WeaponText.text = $"{WeaponSupply}";
        uiManager.GasText.text = $"{GasSupply}";
    }

    public void Update()
    {
        if (gameObject.GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && SupplyPack.activeSelf == false && ((InGameCamera)SceneLoader.CurrentCamera).CurrentCameraMode != CameraInputMode.FPS)
            SupplyPack.SetActive(true);
        else if ((!gameObject.GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") || ((InGameCamera)SceneLoader.CurrentCamera).CurrentCameraMode == CameraInputMode.FPS) && SupplyPack.activeSelf == true)
            SupplyPack.SetActive(false);
    }

    public void DropItem(RoleItems.SupplyItem _itemType)
    {
        Vector3 position = transform.position + (transform.forward * 4f) + new Vector3(0,1.5f,0);
        if (_itemType == RoleItems.SupplyItem.Gas && (GasSupply > 0 || EmVariables.LogisticianMaxSupply == -1)) {
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyGasPrefab", position, transform.rotation, 0);
            UseSupply(_itemType);
        } else if (_itemType == RoleItems.SupplyItem.Weapon && (WeaponSupply > 0 || EmVariables.LogisticianMaxSupply == -1)) {
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyBladePrefab", position, transform.rotation, 0);
            UseSupply(_itemType);
        }
    }

    public void ResetSupplies()
    {
        if (HasInfinite())
            return;

        WeaponSupply = EmVariables.LogisticianMaxSupply;
        GasSupply = EmVariables.LogisticianMaxSupply;

        uiManager.GasText.text = $"{GasSupply}";
        uiManager.GasText.color = uiManager.GetColorForItemCount(GasSupply);
        uiManager.WeaponText.text = $"{WeaponSupply}";
        uiManager.WeaponText.color = uiManager.GetColorForItemCount(WeaponSupply);
    }

    private void UseSupply(RoleItems.SupplyItem _itemType)
    {
        if (HasInfinite())
            return;

        if (_itemType == RoleItems.SupplyItem.Gas) {
            GasSupply--;
            uiManager.GasText.text = $"{GasSupply}";
            uiManager.GasText.color = uiManager.GetColorForItemCount(GasSupply);
        } else if (_itemType == RoleItems.SupplyItem.Weapon) {
            WeaponSupply--;
            uiManager.WeaponText.text = $"{WeaponSupply}";
            uiManager.WeaponText.color = uiManager.GetColorForItemCount(WeaponSupply);
        }
    }

    private bool HasInfinite()
    {
        return EmVariables.LogisticianMaxSupply == -1;
    }

    public void SetSupplies(int maxSupply)
    {
        RPCManager.PhotonView.RPC("SetSuppliesRPC", RpcTarget.AllBuffered, maxSupply);
    }

}