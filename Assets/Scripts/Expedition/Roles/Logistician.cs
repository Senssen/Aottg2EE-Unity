using Characters;
using UnityEngine;
using Photon.Pun;
using Utility;

public class Logistician : MonoBehaviour
{
    private readonly int MaxItemSupply = 4;
    private int WeaponSupply;
    private int GasSupply;
    private LogisticianUiManager uiManager;

    [SerializeField]
    private GameObject SupplyPack;

    public void Start()
    {
        WeaponSupply = MaxItemSupply;
        GasSupply = MaxItemSupply;
        uiManager = GameObject.Find("Expedition UI(Clone)").GetComponent<LogisticianUiManager>();

        uiManager.WeaponText.text = $"{WeaponSupply}";
        uiManager.GasText.text = $"{GasSupply}";
    }

    public void Update()
    {
        if (gameObject.GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && SupplyPack.activeSelf == false)
            SupplyPack.SetActive(true);
        else if (!gameObject.GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && SupplyPack.activeSelf == true)
            SupplyPack.SetActive(false);
    }

    public void DropItem(RoleItems.SupplyItem _itemType)
    {
        Vector3 position = transform.position + (transform.forward * 4f) + new Vector3(0,1.5f,0);
        if (_itemType == RoleItems.SupplyItem.Gas && GasSupply > 0) {
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyGasPrefab", position, transform.rotation, 0);
            UseSupply(_itemType);
        } else if (_itemType == RoleItems.SupplyItem.Weapon && WeaponSupply > 0) {
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyBladePrefab", position, transform.rotation, 0);
            UseSupply(_itemType);
        }
    }

    public void ResetSupplies()
    {
        WeaponSupply = MaxItemSupply;
        GasSupply = MaxItemSupply;

        uiManager.WeaponText.text = $"{WeaponSupply}";
        uiManager.GasText.text = $"{GasSupply}";
    }

    private void UseSupply(RoleItems.SupplyItem _itemType)
    {
        if (_itemType == RoleItems.SupplyItem.Gas) {
            GasSupply--;
            uiManager.GasText.text = $"{GasSupply}";
        } else if (_itemType == RoleItems.SupplyItem.Weapon) {
            WeaponSupply--;
            uiManager.WeaponText.text = $"{WeaponSupply}";
        }
    }

    

}