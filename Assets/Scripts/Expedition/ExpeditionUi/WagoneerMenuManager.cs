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
    private float AnimationDuration = 0.5f;
    private readonly Vector3 FULL_SCALE = new Vector3(0.6f, 1f, 0f);
    private Wagoneer wagoneer;
    private HumanInputSettings _humanInput;

    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
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
        } else if (action == 2) {
            if (wagoneer.CheckIsMounted()) {
                wagoneer.SendRPC("UnmountWagon");
            } else {
                wagoneer.SendRPC("MountWagon");
            }
        } else if (action == 3) {
            // Spawn Supply Station
        } else if (action == 4) {
            // Despawn Supply Station
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
}