using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Settings;
using Characters;
using System.Collections;
using UI;
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

    public bool Ability1Selected = false; // made public so i can set to red on human spawn //
    public bool Ability2Selected = false; // made public so i can set to white on human spawn //
    public bool Ability3Selected = false; // made public so i can set to white on human spawn //

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

    public void OnHoverAbility(int number)
    {
        _human = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Human>();
        _veteran = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Veteran>();
        if (number == 1) {
            if (SettingsManager.InGameCharacterSettings.Special.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special.Value != "None")
            {
                Ability1Selected = true;
                Ability2Selected = false;
                Ability3Selected = false;

                Ability1Selector.color = _selectColor;
            }
        } else if (number == 2) {
            if (SettingsManager.InGameCharacterSettings.Special_2.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special_2.Value != "None")
            {
                Ability1Selected = false;
                Ability2Selected = true;
                Ability3Selected = false;

                Ability2Selector.color = _selectColor;
            }
        } else if (number == 3) {
            if (SettingsManager.InGameCharacterSettings.Special_3.Value.Length > 0 && SettingsManager.InGameCharacterSettings.Special_3.Value != "None")
            {
                Ability1Selected = false;
                Ability2Selected = false;
                Ability3Selected = true;

                Ability3Selector.color = _selectColor;
            }
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

        if (_humanInput.AbilityWheelMenu.GetKeyDown() && !InGameMenu.InMenu())
        {
            KeepSelectedAbilityColor();
            AbilityWheelCanvas.SetActive(true);
            EmVariables.IsOpen = true;
        }
        if (_humanInput.AbilityWheelMenu.GetKeyUp())
        {
            HideAbilityWheel();
        }
    }
}