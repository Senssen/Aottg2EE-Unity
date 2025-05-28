using CustomLogic;
using GameManagers;
using Photon.Pun;
using Settings;
using TMPro;
using UI;
using UnityEngine;

public class WagoneerMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject WagoneerCanvas;
    [SerializeField]
    private TMP_Text AttachWagonButtonText;
    [SerializeField]
    private TMP_Text SupplyStationText;
    [SerializeField]
    private GameObject WagonText;
    [SerializeField]
    private float AnimationDuration = 0.5f;
    private readonly Vector3 FULL_SCALE = new Vector3(0.6f, 1f, 0f);
    private Wagoneer wagoneer;
    private HumanInputSettings _humanInput;
    private InteractionInputSettings _interactionInput;

    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
        _interactionInput = SettingsManager.InputSettings.Interaction;
    }

    void Update()
    {
        if (_humanInput.WagoneerMenu.GetKeyDown() && !InGameMenu.InMenu() && !ChatManager.IsChatActive() && !CustomLogicManager.Cutscene) {
            if (wagoneer == null)
                wagoneer = PhotonExtensions.GetMyHuman().GetComponent<Wagoneer>();

            if (wagoneer.gameObject.GetComponent<PhotonView>().Owner.CustomProperties.ContainsKey("Wagoneer") == false)
                return;
            
            SetWagoneerMenuActive(true);
        }   
    }
    public void HandleClickButton(int action)
    {
        if (action == -1) {
            SetWagoneerMenuActive(false);
        } else if (action == 0) {
            wagoneer.SendRPC("SpawnWagon");
        } else if (action == 1) {
            wagoneer.SendRPC("DespawnWagon");
        } else if (action == 2 && wagoneer.CheckIsMounted() == true) {
            wagoneer.SendRPC("UnmountWagon");
        } else if (action == 2 && wagoneer.CheckIsMounted() == false) {
            wagoneer.SendRPC("MountWagon");
        } else if (action == 3) {
            wagoneer.SendRPC("SpawnStation");
        } else if (action == 4) {
            wagoneer.SendRPC("DespawnStation");
        }
    }

    public void SetWagoneerMenuActive(bool open) {
        if (open == true) {
            WagoneerCanvas.SetActive(true);
            EmVariables.IsOpen = true;

            if (wagoneer.CheckIsMounted()) {
                AttachWagonButtonText.text = "Detach Wagon";
            } else {
                AttachWagonButtonText.text = "Attach Nearby Wagon";
            }

            StartCoroutine(AnimateUI(true));
        } else {
            StartCoroutine(AnimateUI(false));
        }
    }

    System.Collections.IEnumerator AnimateUI(bool isOpening)
    {
        float time = 0f;
        RectTransform rt = WagoneerCanvas.transform.Find("Wagoneer Tab").GetComponent<RectTransform>();
        while (time < AnimationDuration)
        {
            if (isOpening) {
                rt.localScale = Vector3.Lerp(Vector3.zero, FULL_SCALE, time / AnimationDuration);
            } else {
                rt.localScale = Vector3.Lerp(FULL_SCALE, Vector3.zero, time / AnimationDuration);
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (isOpening) {
            rt.localScale = FULL_SCALE;
        } else {
            rt.localScale = Vector3.zero;
            WagoneerCanvas.SetActive(false);
            EmVariables.IsOpen = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) {
            SetWagoneerMenuActive(false);
        }
    }

    public void SetSupplyStationText(bool open)
    {
        if (InGameMenu.InMenu()) {
            SupplyStationText.gameObject.SetActive(false);
            return;
        }

        if (open && SupplyStationText.text.Contains("{{1}}") && SupplyStationText.text.Contains("{{2}}")) {
            SupplyStationText.text = SupplyStationText.text.Replace("{{1}}", $"{_interactionInput.Interact}").Replace("{{2}}", $"{_interactionInput.Interact2}");
        }

        SupplyStationText.gameObject.SetActive(open);
    }

    public void DespawnAllObjectsByName(string name)
    {
        GameObject[] station = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in station)
        {
            if (go.name.Contains(name))
            {
                Destroy(go);
            }
        }
    }

    public void ShowWagonText()
    {
        WagonText.SetActive(true);
        StartCoroutine(AnimateWagonText());
    }

    System.Collections.IEnumerator AnimateWagonText()
    {
        RectTransform rt = WagonText.GetComponent<RectTransform>();
        
        float time = 0f;
        while (time < 0.2f)
        {
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / 0.2f);
            time += Time.deltaTime;
            yield return null;
        }
        rt.localScale = Vector3.one;

        yield return new WaitForSeconds(2f);
        
        time = 0f;
        while (time < 0.2f)
        {
            rt.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time / 0.2f);
            time += Time.deltaTime;
            yield return null;
        }
        rt.localScale = Vector3.zero;
        WagonText.SetActive(false);
    }
}