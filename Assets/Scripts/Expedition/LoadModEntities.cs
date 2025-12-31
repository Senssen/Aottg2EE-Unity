using Settings;
using UnityEngine;

public class LoadModEntities : MonoBehaviour
{
    [SerializeField] private GameObject ExpeditionUIPrefab;
    [SerializeField] private GameObject UniStormSystemPrefab;
    void Start()
    {
        Instantiate(ExpeditionUIPrefab);
        Instantiate(UniStormSystemPrefab);
    }

    public void HandleDynamicWeatherSettings(Camera camera)
    {
        InGameMiscSettings inGameMisc = SettingsManager.InGameUI.Misc;
        bool enabled = inGameMisc.DynamicWeatherEnabled.Value;

        GameObject unistorm = GameObject.Find("UniStorm System(Clone)");
        if (unistorm == null || camera == null)
        {
            Debug.LogWarning($"Could not find UniStorm System or Camera to apply dynamic weather settings. Unistorm: {unistorm != null}, Camera: {camera != null}");
            return;
        }

        Skybox skybox = camera.GetComponent<Skybox>();

        unistorm.SetActive(enabled);
        skybox.enabled = !enabled;
    }
}
