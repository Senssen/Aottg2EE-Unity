using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Settings;
public class LogisticianUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasObj;
    [SerializeField]
    private GameObject SelectScreen;

    private HumanInputSettings _humanInput;

    private RoleItems.SupplyItem selectedItem;
    
    [SerializeField]
    RawImage WeaponImage;
    [SerializeField]
    RawImage GasImage;
    [SerializeField]
    public TMP_Text WeaponText;
    [SerializeField]
    public TMP_Text GasText;


    protected void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
    }

    protected void Update()
    {
        // TODO: this entire thing should definetely not be done this way
        if (!PhotonExtensions.GetMyHuman())
            return;

        if (PhotonExtensions.GetMyHuman().GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && CanvasObj.activeSelf == false) {
            CanvasObj.SetActive(true);
        } else if (!PhotonExtensions.GetMyHuman().GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && CanvasObj.activeSelf == true) {
            CanvasObj.SetActive(false);
        }

        if (_humanInput.LogisticianMenu.GetKey() && SelectScreen.activeSelf == false) {
            OpenSelectScreen();
        } else if (!_humanInput.LogisticianMenu.GetKey() && SelectScreen.activeSelf == true) {
            CloseSelectScreen();
        }
    }

    public void CloseSelectScreen()
    {
        if (selectedItem != RoleItems.SupplyItem.None)
            PhotonExtensions.GetMyHuman().GetComponent<Logistician>().DropItem(selectedItem);

        GasImage.color = Color.white;
        WeaponImage.color = Color.white;
        SelectScreen.SetActive(false);
        EmVariables.SetActive(false);
    }

    public void OpenSelectScreen()
    {
        if (!PhotonExtensions.GetMyHuman().GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician"))
            return;

        SelectScreen.SetActive(true);
        EmVariables.SetActive(true);
    }

    public void SelectItem(RoleItems.SupplyItem type)
    {
        selectedItem = type;
    }

    public void OnHoverSupplyItem(string _itemType)
    {
        if (_itemType == "Gas") {
            SelectItem(RoleItems.SupplyItem.Gas);
            GasImage.color = Color.red;
        } else if (_itemType == "Weapon") {
            SelectItem(RoleItems.SupplyItem.Weapon);
            WeaponImage.color = Color.red;
        }
    }

    public void OnExitSupplyItem(string _itemType)
    {
        if (_itemType == "Gas") {
            GasImage.color = Color.white;
        } else if (_itemType == "Weapon") {
            WeaponImage.color = Color.white;
        }
        SelectItem(RoleItems.SupplyItem.None);
    }
}