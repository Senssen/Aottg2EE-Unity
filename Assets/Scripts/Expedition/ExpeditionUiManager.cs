using Photon.Pun;
using Photon.Realtime;
using Characters;
using Settings;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ExpeditionUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasObj;
    [SerializeField]
    private InputField CoordsInputField;

    [SerializeField]
    private GameObject HorseAutorun;
    [SerializeField]
    private GameObject HumanAutorun;
    
    private GeneralInputSettings _generalInputSettings;


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
        CanvasObj.SetActive(_open);
        EmVariables.SetActive(_open);
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
        GameObject TargetplayerGameObject = PhotonExtensions.GetPlayerFromID(EmVariables.SelectedPlayer.ActorNumber);
        Vector3 Mypos = PhotonExtensions.GetMyPlayer().transform.position;

        switch (setting)
        {
            case 0: //TP all to me 
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    go.GetComponent<Human>().photonView.RPC("moveToRPC", RpcTarget.Others, new object[] { Mypos.x, Mypos.y, Mypos.z });
                }
                break;
            case 1: //TP player to me
                TargetplayerGameObject.GetComponent<Human>().photonView.RPC("moveToRPC", EmVariables.SelectedPlayer, new object[] { Mypos.x, Mypos.y, Mypos.z });
                break;
            case 2: //TP me to player
                GameObject ME = PhotonExtensions.GetMyPlayer();
                ME.transform.position = TargetplayerGameObject.transform.position;
                break;
            case 3: //TP player to coords
                string[] tpCoordsSplit = CoordsInputField.text.Split(' ');
                TargetplayerGameObject.GetComponent<Human>().photonView.RPC("moveToRPC", EmVariables.SelectedPlayer, new object[]
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
        string RoleName = "";
        ExitGames.Client.Photon.Hashtable playerProps = EmVariables.SelectedPlayer.CustomProperties;
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
                RoleName = "Wagon";
                break;
        }

        if (RoleName == string.Empty) return;

        if (playerProps.ContainsKey(RoleName))
            playerProps.Remove(RoleName);
        else
            playerProps.Add(RoleName, true);

        InvokeNameRefresh(playerProps);
    }

    private void InvokeNameRefresh(ExitGames.Client.Photon.Hashtable props)
    {
        EmVariables.SelectedPlayer.SetCustomProperties(props);
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Expedition Menu Player Button");
        foreach (GameObject obj in objects)
        {
            PlayerButton btn = obj.GetComponent<PlayerButton>();
            if (btn && btn.PhotonPlayer.ActorNumber == EmVariables.SelectedPlayer.ActorNumber)
                btn.NameRefresh();
        }
    }
}