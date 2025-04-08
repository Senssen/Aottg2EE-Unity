using Characters;
using Photon.Pun;
using Settings;
using UI;
using UnityEngine;

class Veteran : MonoBehaviour
{
    private Human human;
    private VeteranManager _veteranManager;
    public bool isVeteranSet = false;
    void Start()
    {
        _veteranManager = FindFirstObjectByType<VeteranManager>();

        human = gameObject.GetComponent<Human>();

        _veteranManager.Ability1Selected = true;
        _veteranManager.Ability2Selected = false;
        _veteranManager.Ability3Selected = false;
    }

    void Update()
    {
        SetupVeteran();
    }

    public void SetMyHuman(Human _human)
    {
        human = _human;
    }

    public void SetAllSpecials(string special1, string special2, string special3)
    {
        human.Special = HumanSpecials.GetSpecialUseable(human, special1);
        human.Special_2 = HumanSpecials.GetSpecialUseable(human, special2);
        human.Special_3 = HumanSpecials.GetSpecialUseable(human, special3);
    }

    public void SwitchCurrentSpecial(string special, int newSpecial)
    {
        if (human.CurrentSpecial != special)
        {
            bool canAnimationReset = (human.State != HumanState.Die && human.State != HumanState.Grab && human.State != HumanState.MountingHorse && human.State != HumanState.Stun && human.State != HumanState.GroundDodge);
            if (canAnimationReset)
                human.State = HumanState.Idle;

            human.CurrentSpecial = special;

            human.Special = HumanSpecials.GetSpecialUseable(human, special);
            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon(HumanSpecials.GetSpecialIcon(special));

            if (newSpecial == 1)
            {
                human.Special_2 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special_2.Value);
                human.Special_3 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special_3.Value);

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special_2.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_3.Value;
            }
            if (newSpecial == 2)
            {
                human.Special_2 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special.Value);
                human.Special_3 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special_3.Value);

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_3.Value;
            }
            if (newSpecial == 3)
            {
                human.Special_2 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special.Value);
                human.Special_3 = HumanSpecials.GetSpecialUseable(human, SettingsManager.InGameCharacterSettings.Special_2.Value);

                human.SideSpecial_1 = SettingsManager.InGameCharacterSettings.Special.Value;
                human.SideSpecial_2 = SettingsManager.InGameCharacterSettings.Special_2.Value;
            }

            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon_2(HumanSpecials.GetSpecialIcon(human.SideSpecial_1));
            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.SetSpecialIcon_3(HumanSpecials.GetSpecialIcon(human.SideSpecial_2));
        }
    }

    #region Veteran Specific

    public void SetupVeteran()
    {
        if (!isVeteranSet)
        {
            isVeteranSet = true; // for only setting up once

            if (human.Setup.Weapon == HumanWeapon.Blade || human.Setup.Weapon == HumanWeapon.AHSS || human.Setup.Weapon == HumanWeapon.APG)
            {
                var tsInfo = CharacterData.HumanWeaponInfo["Thunderspear"];
                human.Setup.Weapon_2 = HumanWeapon.Thunderspear;
                human.Weapon_2 = new ThunderspearWeapon(human, 12, 2, 1.5f, 7f, 500f, 0.5f, 0.12f, tsInfo);
            }
            if (human.Setup.Weapon == HumanWeapon.Thunderspear)
            {
                human.Setup.Weapon_2 = HumanWeapon.Blade;
                human.Weapon_2 = new BladeWeapon(human, 175f, 4); // give more if need be
            }
        }
    }

    public void SwitchVeteranLoadout()
    {
        (human.Weapon, human.Weapon_2) = (human.Weapon_2, human.Weapon);
        (human.Setup.Weapon, human.Setup.Weapon_2) = (human.Setup.Weapon_2, human.Setup.Weapon);

        /* human.Setup.CreateParts();
        human.Setup.CreateWeapon();
        human.Setup.Create3dmg();
        human.Setup.CreateCape();
        human.LateUpdateFPS(); */
        human.Setup.CreateWeapon();

        if (human.Weapon is BladeWeapon)
        {
            if (((BladeWeapon)human.Weapon).CurrentDurability <= 0)
                human.ToggleBlades(false);
        }
        if (human.Weapon is ThunderspearWeapon)
        {
            if (((ThunderspearWeapon)human.Weapon).RoundLeft <= 0)
            {
                human.SetThunderspears(false, false);
            }
        }

        ReloadGearSkin();

        HUDBottomHandler _hudBottomHandler = FindFirstObjectByType<HUDBottomHandler>();
        if (_hudBottomHandler != null)
        {
            _hudBottomHandler.SetBottomHUD(GetComponent<Human>());
        }
    }

    private void ReloadGearSkin()
    {
        if (human.IsMine())
        {
            if (SettingsManager.CustomSkinSettings.Human.SkinsEnabled.Value)
            {
                HumanCustomSkinSet set = (HumanCustomSkinSet)SettingsManager.CustomSkinSettings.Human.GetSelectedSet();

                string url = string.Join(",", new string[] { null, null, null, null, null,
                    null, null, null, null, null, null, null,
                    set.WeaponTrail.Value, set.ThunderspearL.Value, set.ThunderspearR.Value, null, null,
                    null, null, null, null, null });

                /* string url = string.Join(",", new string[] { null, null, null, null, null,
                    null, null, null, set.GearL.Value, set.GearR.Value, null, null,
                    set.WeaponTrail.Value, set.ThunderspearL.Value, set.ThunderspearR.Value, null, null,
                    null, null }); */

                int viewID = -1;
                if (human.Horse != null)
                {
                    viewID = human.Horse.gameObject.GetPhotonView().ViewID;
                }
                human.Cache.PhotonView.RPC("LoadSkinRPC", RpcTarget.AllBuffered, new object[] { viewID, url });
            }
        }
    }

    #endregion
}
