using System.Collections;
using System.Collections.Generic;
using UniStorm;
using UnityEngine;
using Settings;
using GameManagers;
using ApplicationManagers;
using Photon.Pun;
using Utility;

public class DynamicWeatherManager : MonoBehaviour
{
    public static UniStormSystem uniStormSystem;

    public static void InitializeUniStorm()
    {
        if (PhotonNetwork.IsMasterClient && SettingsManager.InGameUI.Misc.DynamicWeatherEnabled.Value)
        {
            SetupUniStorm();
        }
        else
        {
            ChatManager.AddLine("implement multiplayer later");
        }
    }

    private static void SetupUniStorm()
    {
        if (uniStormSystem != null)
            Destroy(uniStormSystem.gameObject);

        Camera camera = SceneLoader.CurrentCamera.Camera;
        GameObject uniStormGo = ResourceManager.InstantiateAsset<GameObject>(ResourcePaths.DynamicWeather, "UniStorm System");
        uniStormSystem = uniStormGo.GetComponent<UniStormSystem>();

        if (camera == null)
        {
            ChatManager.AddLine("<color=red>Could not find game camera to disable for UniStorm!</color>");
            return;
        }

        Skybox skybox = camera.GetComponent<Skybox>();
        GameObject daylight = GameObject.Find("Daylight");

        skybox.enabled = false;
        if (daylight != null)
            daylight.SetActive(false);
        else
            ChatManager.AddLine("<color=yellow>Could not find daylight on the scene!</color>");
    }
}
