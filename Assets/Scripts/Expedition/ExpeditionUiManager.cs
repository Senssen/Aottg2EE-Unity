using Photon.Pun;
using Characters;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;
using GameManagers;
public class ExpeditionUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasObj;
    [SerializeField]
    private GameObject TabsParent;
    [SerializeField]
    private GameObject PlayerListTab;
    [SerializeField]
    private GameObject SettingsTab;
    [SerializeField]
    private ScrollRect PlayerListScrollArea;
    [SerializeField]
    private float AnimationDuration = 0.5f;
    [SerializeField]
    private InputField CoordsInput;
    [SerializeField]
    private InputField LogisticianMaxSupplyInput;

    [SerializeField]
    private GameObject HorseAutorun;
    [SerializeField]
    private GameObject HumanAutorun;

    private GeneralInputSettings _generalInputSettings;

    [SerializeField]
    private TMP_Text NonLethalCannonText;


    private void Start()
    {
        _generalInputSettings = SettingsManager.InputSettings.General;
    }

    private void Update()
    {
        if (CanvasObj.activeSelf && _generalInputSettings.Pause.GetKeyDown())
            ControlMenu(false);
    }

    public void ControlMenu(bool _open)
    {
        EmVariables.SetActive(_open);
        if (_open == true)
        {
            CanvasObj.SetActive(true);
            InvokeNameRefresh();
            PlayerListTab.SetActive(true);
            SetNonLethalCannonsText();
            StartCoroutine(AnimateUI(true));
        }
        else
        {
            StartCoroutine(AnimateUI(false));
            GetComponent<PlayerListManager>().ResetSelectedButton();
        }
    }

    System.Collections.IEnumerator AnimateUI(bool isOpening)
    {
        float time = 0f;
        RectTransform rt = TabsParent.GetComponent<RectTransform>();
        while (time < AnimationDuration)
        {
            if (isOpening)
            {
                rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / AnimationDuration);
            }
            else
            {
                rt.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time / AnimationDuration);
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (isOpening)
        {
            rt.localScale = Vector3.one;
            PlayerListScrollArea.verticalNormalizedPosition = 1f; // added this line because otherwise the player list might scroll randomly on first open
        }
        else
        {
            rt.localScale = Vector3.zero;
            CanvasObj.SetActive(false); // carried these canvas active settings here because otherwise they will be inactive before the coroutine ends
            PlayerListTab.SetActive(false);
            SettingsTab.SetActive(false);
        }
    }

    public void UpdatePlayerListVerticalScroll()
    {
        PlayerListScrollArea.vertical = GetComponent<PlayerListManager>().PlayerListings.Count >= 5;
    }

    public void ControlHorseAutorun(bool _open)
    {
        if (HorseAutorun.activeSelf != _open)
            HorseAutorun.SetActive(_open);

        if (_open)
            HumanAutorun.SetActive(false);
    }
    public void ControlHumanAutorun(bool _open)
    {
        if (HumanAutorun.activeSelf != _open)
            HumanAutorun.SetActive(_open);

        if (_open)
            HorseAutorun.SetActive(false);
    }
    public void OnTPPlayerButtonClick(int setting)
    {
        if (EmVariables.SelectedPlayer == null)
            return;

        GameObject TargetplayerGameObject = PhotonExtensions.GetPlayerFromID(EmVariables.SelectedPlayer.ActorNumber);
        Vector3 Mypos = PhotonExtensions.GetMyPlayer().transform.position;

        switch (setting)
        {
            case 0: //TP all to me 
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    go.GetComponent<Human>().photonView.RPC("MoveToRPC", RpcTarget.Others, new object[] { Mypos.x, Mypos.y, Mypos.z });
                }
                break;
            case 1: //TP player to me
                TargetplayerGameObject.GetComponent<Human>().photonView.RPC("MoveToRPC", EmVariables.SelectedPlayer, new object[] { Mypos.x, Mypos.y, Mypos.z });
                break;
            case 2: //TP me to player
                GameObject ME = PhotonExtensions.GetMyPlayer();
                ME.transform.position = TargetplayerGameObject.transform.position;
                break;
            case 3: //TP player to coords
                string[] tpCoordsSplit = CoordsInput.text.Split(' ');
                TargetplayerGameObject.GetComponent<Human>().photonView.RPC("MoveToRPC", EmVariables.SelectedPlayer, new object[]
                {
                    float.Parse(tpCoordsSplit[0]),
                    float.Parse(tpCoordsSplit[1]),
                    float.Parse(tpCoordsSplit[2])
                });
                break;
        }
    }

    public void GiveRoles(int Role)
    {
        if (EmVariables.SelectedPlayer == null)
            return;

        string RoleName = "";
        switch (Role)
        {
            case 0:
                RoleName = "Logistician";
                break;
            case 1:
                RoleName = "Cannoneer";
                break;
            case 2:
                RoleName = "Carpenter";
                break;
            case 3:
                RoleName = "Veteran";
                break;
            case 4:
                RoleName = "Wagoneer";
                break;
        }

        if (RoleName == string.Empty) return;

        if (EmVariables.SelectedPlayer.CustomProperties.ContainsKey(RoleName))
        {
            Hashtable props = new Hashtable();
            props[RoleName] = null;
            EmVariables.SelectedPlayer.SetCustomProperties(props);
        }
        else
        {
            Hashtable props = new Hashtable();
            props[RoleName] = true;
            EmVariables.SelectedPlayer.SetCustomProperties(props);
        }

        InvokeNameRefresh(EmVariables.SelectedPlayer.ActorNumber);
    }

    public void InvokeNameRefresh(int actorNumber)
    {
        foreach (PlayerButton btn in GetComponent<PlayerListManager>().PlayerListings)
        {
            if (btn && btn.PhotonPlayer.ActorNumber == actorNumber)
                btn.NameRefresh();
        }
    }

    public void InvokeNameRefresh()
    {
        foreach (PlayerButton btn in GetComponent<PlayerListManager>().PlayerListings)
        {
            btn.NameRefresh();
        }
    }

    public void ControlTabButton(int option)
    {
        if (option == 0)
        {
            ControlMenu(false);
        }
        else if (option == 1)
        {
            PlayerListTab.SetActive(true);
            SettingsTab.SetActive(false);
        }
        else if (option == 2)
        {
            SettingsTab.SetActive(true);
            LogisticianMaxSupplyInput.text = EmVariables.LogisticianMaxSupply.ToString();
            PlayerListTab.SetActive(false);
        }
        else
        {
            Debug.Log($"No action specified for option {option}");
        }
    }

    public void SetLogisticianMaxSupply()
    {
        if (int.TryParse(LogisticianMaxSupplyInput.text, out int value))
        {
            if (value < -1)
            {
                Debug.LogWarning("The MC may only set the logistician value to numbers greater than or equal to -1. -1 means infinite supply.");
            }

            RPCManager.PhotonView.RPC("SetSuppliesRPC", RpcTarget.AllBuffered, value);
        }
        else
        {
            Debug.LogError($"The value set was not an integer: {LogisticianMaxSupplyInput.text}");
        }
    }

    public void HandleSetNonLethalCannons()
    {
        SetNonLethalCannons(!EmVariables.NonLethalCannons);
        SetNonLethalCannonsText();
    }

    public void SetNonLethalCannons(bool _isNonLethal)
    {
        RPCManager.PhotonView.RPC("SetNonLethalCannonsRPC", RpcTarget.AllBuffered, _isNonLethal);
    }

    private void SetNonLethalCannonsText()
    {
        NonLethalCannonText.text = $"Non-lethal Cannons: {EmVariables.NonLethalCannons}";
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ControlMenu(false);
        }
    }
}