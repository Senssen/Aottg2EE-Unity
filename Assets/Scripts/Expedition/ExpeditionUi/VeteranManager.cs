using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Settings;
using Characters;
using System.Collections;
using UI;
using GameManagers;
using CustomLogic;
public class VeteranManager : MonoBehaviour
{
    private readonly Color _selectColor = new Color(0.525f, 0.164f, 0.227f);

    [SerializeField]
    private GameObject AbilityWheelCanvas;
    [SerializeField]
    private Image Ability1Image; 
    [SerializeField]
    private Image Ability2Image;
    [SerializeField]
    private Image Ability3Image;
    [SerializeField]
    private RawImage Ability1Selector;
    [SerializeField]
    private RawImage Ability2Selector;
    [SerializeField]
    private RawImage Ability3Selector;

    [SerializeField]
    private GameObject LoadoutParent;
    [SerializeField]
    private Image LoadoutImage;
    [SerializeField]
    private RawImage LoadoutSelector;

    public bool Ability1Selected = false; // made public so i can set to red on human spawn //
    public bool Ability2Selected = false; // made public so i can set to white on human spawn //
    public bool Ability3Selected = false; // made public so i can set to white on human spawn //
    public bool LastHoveredLoadout = false;

    private HumanInputSettings _humanInput;
    private Human _human;
    private Veteran _veteran;

    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
    }

    void Update()
    {
        AbilityWheelUpdate();
        UpdateLoadoutVisibility();
    }
    public void SetWheelImages()
    {
        if (SettingsManager.InGameCharacterSettings.Special.Value.Length == 0 || SettingsManager.InGameCharacterSettings.Special.Value == "None")
        {
            Ability1Image.sprite = LoadSprite("No");
        }
        else
        {
            Ability1Image.sprite = LoadSprite(SettingsManager.InGameCharacterSettings.Special.Value);
        }

        if (SettingsManager.InGameCharacterSettings.Special_2.Value.Length == 0 || SettingsManager.InGameCharacterSettings.Special_2.Value == "None")
        {
            Ability2Image.sprite = LoadSprite("No");
        }
        else
        {
            Ability2Image.sprite = LoadSprite(SettingsManager.InGameCharacterSettings.Special_2.Value);
        }

        if (SettingsManager.InGameCharacterSettings.Special_3.Value.Length == 0  || SettingsManager.InGameCharacterSettings.Special_3.Value == "None")
        {
            Ability3Image.sprite = LoadSprite("No");
        }
        else
        {
            Ability3Image.sprite = LoadSprite(SettingsManager.InGameCharacterSettings.Special_3.Value);
        }
    }

    private Sprite LoadSprite(string spriteName)
    {
        string path = "UI/Icons/Specials/" + spriteName.Replace(" ", "") + "SpecialIcon";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError("Sprite not found at path: " + path);
        }
        return sprite;
    }

    private Sprite LoadSpriteForLoadout(string spriteName) {
        string path = "UI/Icons/EmIcons/Vet" + spriteName;
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError("Sprite not found at path: " + path);
        }
        return sprite;
    }

    public void OnHoverAbility(int number)
    {
        if (number == 1) {
            if (SettingsManager.InGameCharacterSettings.Special.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special.Value != "None")
            {
                Ability1Selected = true;
                Ability2Selected = false;
                Ability3Selected = false;
                LastHoveredLoadout = false;

                Ability1Selector.color = _selectColor;
            }
        } else if (number == 2) {
            if (SettingsManager.InGameCharacterSettings.Special_2.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special_2.Value != "None")
            {
                Ability1Selected = false;
                Ability2Selected = true;
                Ability3Selected = false;
                LastHoveredLoadout = false;

                Ability2Selector.color = _selectColor;
            }
        } else if (number == 3) {
            if (SettingsManager.InGameCharacterSettings.Special_3.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special_3.Value != "None")
            {
                Ability1Selected = false;
                Ability2Selected = false;
                Ability3Selected = true;
                LastHoveredLoadout = false;

                Ability3Selector.color = _selectColor;
            }
        } else if (number == 4) {
            Ability1Selected = false;
            Ability2Selected = false;
            Ability3Selected = false;
            LastHoveredLoadout = true; 
            LoadoutImage.color = _selectColor;
        }
    }

    public void OnHoverExitAbility(int number)
    {
        if (number == 1) {
            Ability1Selected = false;
            Ability1Selector.color = Color.white;
        } else if (number == 2) {
            Ability2Selected = false;
            Ability2Selector.color = Color.white;
        } else if (number == 3) {
            Ability3Selected = false;
            Ability3Selector.color = Color.white;
        } else if (number == 4) {
            LastHoveredLoadout = false; 
            LoadoutImage.color = Color.white;
        }
    }

    private void HideAbilityWheel()
    {
        AbilityWheelCanvas.SetActive(false);
        EmVariables.IsOpen = false;
        Ability1Image.color = Color.white;
        Ability2Image.color = Color.white;
        Ability3Image.color = Color.white;
        Ability1Selector.color = Color.white;
        Ability2Selector.color = Color.white;
        Ability3Selector.color = Color.white;

        LoadoutSelector.color = new Color(184f/255f, 184f/255f, 184f/255f);
        LoadoutImage.color = Color.white;

        if (Ability1Selected) {
            if (_human.CurrentSpecial != SettingsManager.InGameCharacterSettings.Special.Value) {
                _veteran.SwitchCurrentSpecial(SettingsManager.InGameCharacterSettings.Special.Value, 1);
                _veteran.PlayAbilitySelectSound();
            }
        } else if (Ability2Selected) {
            if (_human.CurrentSpecial != SettingsManager.InGameCharacterSettings.Special_2.Value) {
                _veteran.SwitchCurrentSpecial(SettingsManager.InGameCharacterSettings.Special_2.Value, 2);
                _veteran.PlayAbilitySelectSound();
            }
        } else if (Ability3Selected) {
            if (_human.CurrentSpecial != SettingsManager.InGameCharacterSettings.Special_3.Value) {
                _veteran.SwitchCurrentSpecial(SettingsManager.InGameCharacterSettings.Special_3.Value, 3);
                _veteran.PlayAbilitySelectSound();
            }
        } else if (LastHoveredLoadout) {
            _veteran.SwitchVeteranLoadout();
            LastHoveredLoadout = false;
        }
    }

    public void KeepSelectedAbilityColor()
    {
        if (Ability1Selected)
        {
            Ability1Image.color = _selectColor;
        }
        else
        {
            Ability1Image.color = Color.white;
        }

        if (Ability2Selected)
        {
            Ability2Image.color = _selectColor;
        }
        else
        {
            Ability2Image.color = Color.white;
        }

        if (Ability3Selected)
        {
            Ability3Image.color = _selectColor;
        }
        else
        {
            Ability3Image.color = Color.white;
        }
    }

    private void AbilityWheelUpdate()
    {
        if (PhotonExtensions.GetMyPlayer() == null)
            return;
        
        if ((_human == null || _human.Dead) && AbilityWheelCanvas.activeSelf)
        {
            HideAbilityWheel();
            return;
        }

        if (_humanInput.AbilityWheelMenu.GetKeyDown() && !InGameMenu.InMenu() && !ChatManager.IsChatActive() && !CustomLogicManager.Cutscene)
        {
            if (_human == null)
                _human = PhotonExtensions.GetMyHuman().GetComponent<Human>();
            if (_veteran == null)
                _veteran = PhotonExtensions.GetMyHuman().GetComponent<Veteran>();

            KeepSelectedAbilityColor();
            AbilityWheelCanvas.SetActive(true);
            EmVariables.IsOpen = true;

            if (_human.Weapon_2 != null && PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Veteran"))
                SetLoadoutImages();
        }
        if (_humanInput.AbilityWheelMenu.GetKeyUp())
        {
            HideAbilityWheel();
        }
    }

    private void UpdateLoadoutVisibility()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Veteran"))
        {
            if (LoadoutParent.activeInHierarchy == false)
                LoadoutParent.SetActive(true);
        }
        else
        {
            LoadoutParent.SetActive(false);
        }
    }

    private void SetLoadoutImages()
    {
        if(_human.Setup.Weapon_2 == HumanWeapon.Blade)
            LoadoutImage.sprite = LoadSpriteForLoadout("Blades");
        if(_human.Setup.Weapon_2 == HumanWeapon.AHSS)
            LoadoutImage.sprite = LoadSpriteForLoadout("AHSS");
        if(_human.Setup.Weapon_2 == HumanWeapon.APG)
            LoadoutImage.sprite = LoadSpriteForLoadout("APG");
        if(_human.Setup.Weapon_2 == HumanWeapon.Thunderspear)
            LoadoutImage.sprite = LoadSpriteForLoadout("TS");
    }
}