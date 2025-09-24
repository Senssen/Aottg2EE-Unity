using Characters;
using Settings;
using UI;
using UnityEngine;

public class SupplyStation : MonoBehaviour
{
    private HumanInputSettings _humanInput;
    private InteractionInputSettings _interactionInput;
    private WagoneerMenuManager _wagoneerMenu;

    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
        _interactionInput = SettingsManager.InputSettings.Interaction;
        GameObject _expeditionUi = GameObject.Find("Expedition UI(Clone)");
        if (_expeditionUi) {
            _wagoneerMenu = _expeditionUi.GetComponent<WagoneerMenuManager>();
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Human _human) && _human.photonView.IsMine) {
            if (_human.NeedRefill(true))
            {
                if (_humanInput.AutoRefillGas.Value == true)
                {
                    _human.Refill();
                }
                else if (_interactionInput.Interact.GetKeyDown())
                {
                    _human.Refill();
                }
            }

            if (_interactionInput.Interact2.GetKeyDown()) {
                ((InGameMenu)UIManager.CurrentMenu).ShowCharacterChangeMenu();
            }

            SetSupplyStationText(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Human _human) && _human.photonView.IsMine) {
            SetSupplyStationText(false);
        }
    }

    private void SetSupplyStationText(bool open)
    {
        if (_wagoneerMenu != null) {
            _wagoneerMenu.SetSupplyStationText(open);
        }
    }
}