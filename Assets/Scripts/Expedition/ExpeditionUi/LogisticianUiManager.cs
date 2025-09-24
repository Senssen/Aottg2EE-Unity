using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Settings;
using UI;
using GameManagers;
using CustomLogic;
public class LogisticianUiManager : MonoBehaviour
{
    private readonly Color _selectColor = new Color(0.525f, 0.164f, 0.227f);

    [SerializeField]
    private GameObject CanvasObj;
    [SerializeField]
    private GameObject SelectScreen;

    private HumanInputSettings _humanInput;

    private RoleItems.SupplyItem selectedItem = RoleItems.SupplyItem.None;

    [SerializeField]
    RawImage WeaponImage;
    [SerializeField]
    RawImage GasImage;
    [SerializeField]
    public TMP_Text WeaponText;
    [SerializeField]
    public TMP_Text GasText;
    [SerializeField]
    private AudioSource SelectSound;


    protected void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
    }

    protected void Update()
    {
        // TODO: this entire thing should definetely not be done this way
        if (!PhotonExtensions.GetMyHuman())
            return;

        if (PhotonExtensions.GetMyHuman().GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && CanvasObj.activeSelf == false)
        {
            CanvasObj.SetActive(true);
        }
        else if (!PhotonExtensions.GetMyHuman().GetPhotonView().Owner.CustomProperties.ContainsKey("Logistician") && CanvasObj.activeSelf == true)
        {
            CanvasObj.SetActive(false);
        }

        if (_humanInput.LogisticianMenu.GetKey() && SelectScreen.activeSelf == false && !InGameMenu.InMenu() && !ChatManager.IsChatActive() && !CustomLogicManager.Cutscene)
        {
            OpenSelectScreen();
        }
        else if (!_humanInput.LogisticianMenu.GetKey() && SelectScreen.activeSelf == true)
        {
            CloseSelectScreen();
        }
    }

    public void CloseSelectScreen()
    {
        if (selectedItem != RoleItems.SupplyItem.None)
            PhotonExtensions.GetMyHuman().GetComponent<Logistician>().DropItem(selectedItem);

        SelectItem(RoleItems.SupplyItem.None);
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
        if (_itemType == "Gas")
        {
            SelectItem(RoleItems.SupplyItem.Gas);
            GasImage.color = _selectColor;
        }
        else if (_itemType == "Weapon")
        {
            SelectItem(RoleItems.SupplyItem.Weapon);
            WeaponImage.color = _selectColor;
        }

        SelectSound.Play();
    }

    public void OnExitSupplyItem(string _itemType)
    {
        if (_itemType == "Gas")
        {
            GasImage.color = Color.white;
        }
        else if (_itemType == "Weapon")
        {
            WeaponImage.color = Color.white;
        }
        SelectItem(RoleItems.SupplyItem.None);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && SelectScreen.activeSelf)
        {
            CloseSelectScreen();
        }
    }
    
    public Color GetColorForItemCount(int count)
    {
        if (count == -1)
            return new Color(0.475f, 0.592f, 0.318f);

        if (count == 0) {
            return new Color(0.514f, 0.231f, 0.267f);
        } else if ((count / EmVariables.LogisticianMaxSupply > 0f && count / EmVariables.LogisticianMaxSupply < .3f) || (count > 0 && count <= 2)) {
            return new Color(0.8f, 0.608f, 0.278f);
        } else {
            return new Color(0.475f, 0.592f, 0.318f);
        }
    }
}