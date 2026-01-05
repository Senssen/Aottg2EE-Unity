using System.Collections;
using System.Collections.Generic;
using UniStorm;
using UnityEngine;
using Settings;
using GameManagers;
using ApplicationManagers;
using Photon.Pun;
using Utility;
using Map;

public class DynamicWeatherManager : MonoBehaviour
{
    private static DynamicWeatherManager _instance;
    public static UniStormSystem uniStormSystem;

    void Awake()
    {
        _instance = this;
    }

    public static void InitializeUniStorm()
    {
        if (SceneLoader.SceneName != SceneName.InGame)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (SettingsManager.InGameUI.Misc.DynamicWeatherEnabled.Value)
            {
                SetupUniStorm();
                uniStormSystem.OnWeatherChangeEvent.AddListener(SetLobbyWeather);
                uniStormSystem.OnSliderEndEvent.AddListener(SetLobbyTime);
            }
        }
        else
        {
            RPCManager.PhotonView.RPC(nameof(RPCManager.RequestTimeAndWeatherRPC), RpcTarget.MasterClient);
        }
    }

    public static void SetupUniStorm(int hour = -1, int minute = -1, string weatherName = "")
    {
        if (uniStormSystem != null)
            Destroy(uniStormSystem.gameObject);

        GameObject uniStormGo = ResourceManager.InstantiateAsset<GameObject>(ResourcePaths.DynamicWeather, "UniStorm System");
        uniStormSystem = uniStormGo.GetComponent<UniStormSystem>();

        if (weatherName.Length != 0)
            uniStormSystem.ChangeWeatherByName(weatherName, useTransition: false);

        if (hour >= 0 && minute >= 0)
            uniStormSystem.SetTime(hour, minute);

        if (!PhotonNetwork.IsMasterClient)
            uniStormSystem.WeatherGeneration = UniStormSystem.EnableFeature.Disabled;

        DisableCameraAndSkybox();
    }

    private static void DisableCameraAndSkybox()
    {
        if (SceneLoader.CurrentCamera.Camera == null)
        {
            ChatManager.AddLine("<color=red>Could not find game camera to disable for UniStorm!</color>");
            return;
        }

        Skybox skybox = SceneLoader.CurrentCamera.Camera.GetComponent<Skybox>();
        skybox.enabled = false;

        _instance.StartCoroutine(_instance.DisableDaylight());
    }

    private IEnumerator DisableDaylight()
    {
        if (MapManager.MapLoaded == false)
            yield return null;

        GameObject daylight = GameObject.Find("Daylight");
        if (daylight != null)
            daylight.SetActive(false);
        else
            ChatManager.AddLine("<color=yellow>Could not find daylight on the scene!</color>");
    }

    private static void SetLobbyWeather()
    {
        string name = uniStormSystem.CurrentWeatherType.WeatherTypeName;
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormWeatherRPC), RpcTarget.Others, new object[] { name });
    }

    private static void SetLobbyTime()
    {
        uniStormSystem.CalculateHourAndMinute();
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormTimeRPC), RpcTarget.Others, new object[] { uniStormSystem.Hour, uniStormSystem.Minute, uniStormSystem.m_TimeFloat });
    }
}
