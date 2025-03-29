using System.Collections;
using System.Collections.Generic;
using Characters;
using Settings;
using UI;
using UnityEngine;

class Veteran : MonoBehaviour
{
    private Human human;
    private VeteranManager _veteranManager;
    [SerializeField]
    private AudioSource SelectAbilityAudio;
    void Start()
    {
        _veteranManager = FindFirstObjectByType<VeteranManager>();

        human = gameObject.GetComponent<Human>();

        _veteranManager.SetWheelImages(); // setting up the ability wheel UI //
        _veteranManager.Ability1Selected = true; // setting up the ability wheel UI //
        _veteranManager.Ability2Selected = false; // setting up the ability wheel UI //
        _veteranManager.Ability3Selected = false; // setting up the ability wheel UI //
    }

    public void SetMyHuman(Human _human)
    {
        human = _human;
    }

    public void SetAllSpecials(string special1, string special2, string special3)
    {
        human.SpecialsArray = new BaseUseable[]
        {
                HumanSpecials.GetSpecialUseable(human, special1),
                (special2.Length > 0) ? HumanSpecials.GetSpecialUseable(human, special2) : null,
                (special3.Length > 0) ? HumanSpecials.GetSpecialUseable(human, special3) : null
        };

        // add the icons for all specials at some point //
    }

    public void SwitchCurrentSpecial(string special, int newSpecial)
    {
        if (human.CurrentSpecial != special)
        {
            bool canAnimationReset = (human.State != HumanState.Die && human.State != HumanState.Grab && human.State != HumanState.MountingHorse && human.State != HumanState.Stun && human.State != HumanState.GroundDodge);
            if (canAnimationReset)
                human.State = HumanState.Idle;

            human.CurrentSpecial = special;
            human.Special = human.SpecialsArray[newSpecial - 1];
            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon(HumanSpecials.GetSpecialIcon(special));

            if (newSpecial == 1)
            {
                human.Special_2 = human.SpecialsArray[1];
                human.Special_3 = human.SpecialsArray[2];

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special_2.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_3.Value;
            }
            if (newSpecial == 2)
            {
                human.Special_2 = human.SpecialsArray[0];
                human.Special_3 = human.SpecialsArray[2];

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_3.Value;
            }
            if (newSpecial == 3)
            {
                human.Special_2 = human.SpecialsArray[0];
                human.Special_3 = human.SpecialsArray[1];

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_2.Value;
            }

            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon_2(HumanSpecials.GetSpecialIcon(human.SideSpecial_1));
            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon_3(HumanSpecials.GetSpecialIcon(human.SideSpecial_2));
        }
    }
    
    public void PlayAbilitySelectSound()
    {
        SelectAbilityAudio.Play();
    }
}
