using System.Collections;
using System.Collections.Generic;
using Characters;
using CustomLogic;
using GameManagers;
using Settings;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class FlareWheelManager : MonoBehaviour
{
    private readonly Color _selectColor = new Color(0.525f, 0.164f, 0.227f);
    private HumanInputSettings _humanInput;
    int? hoveredFlare = null;
    [SerializeField]
    private GameObject FlareWheelCanvas;
    [SerializeField]
    private AudioSource SelectAudio;
    [SerializeField]
    private RawImage[] SelectorImages = new RawImage[8];
    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
    }

    void Update()
    {
        if (_humanInput.FlareWheelMenu.GetKeyDown()) {
            ControlFlareWheel(true);
        } else if (_humanInput.FlareWheelMenu.GetKeyUp()) {
            ControlFlareWheel(false);
        }
    }

    public void OnHoverFlare(int i)
    {
        if (i >= 1 && i <= 8) {
            hoveredFlare = i;
            SelectorImages[i-1].color = _selectColor;
            SelectAudio.Play();
        } else {
            hoveredFlare = null;
        }
    }

    public void OnHoverExitFlare(int i)
    {
        if (i >= 1 && i <= 8) {
            SelectorImages[i-1].color = Color.white;
        }
        
        hoveredFlare = null;
    }

    private void ControlFlareWheel(bool b)
    {
        if (b == true && !InGameMenu.InMenu() && !ChatManager.IsChatActive() && !CustomLogicManager.Cutscene) {
            FlareWheelCanvas.SetActive(true);
            EmVariables.SetActive(true);
        } else {
            FlareWheelCanvas.SetActive(false);
            EmVariables.SetActive(false);

            for (int i = 0; i < SelectorImages.Length; ++i) {
                SelectorImages[i].color = Color.white;
            }

            if (hoveredFlare != null && hoveredFlare >= 1 && hoveredFlare <= 8) {
                if (hoveredFlare == 7 || hoveredFlare == 8)
                    return; // do this for now til I add acoustic and flash flares

                PhotonExtensions.GetMyHuman().GetComponent<Human>().UseItem((int)hoveredFlare - 1);
            }
        }
    }
    
}