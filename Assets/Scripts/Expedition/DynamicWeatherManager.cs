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
            bool enabled = SettingsManager.InGameUI.Misc.DynamicWeatherEnabled.Value;
            HandleRoomProperty(enabled);
            if (enabled)
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
            uniStormSystem.IsMasterClient = false;

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
    }

    private static void HandleRoomProperty(bool enabled)
    {
        var props = new ExitGames.Client.Photon.Hashtable();

        if (enabled)
            props["DynamicWeatherEnabled"] = true;
        else
            props["DynamicWeatherEnabled"] = null; // remove

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public static void ChangeCloudType()
    {
        bool isVolumetric = SettingsManager.InGameUI.Misc.VolumetricClouds.Value;
        UniStormSystem.CloudTypeEnum cloudType = isVolumetric ? UniStormSystem.CloudTypeEnum.Volumetric : UniStormSystem.CloudTypeEnum._2D;
        uniStormSystem.UpdateCloudRenderSettings(cloudType);
    }

    public static void ChangeCloudQuality()
    {
        UniStormSystem.CloudQualityEnum cloudQuality = (UniStormSystem.CloudQualityEnum)SettingsManager.InGameUI.Misc.CloudQuality.Value;
        uniStormSystem.UpdateCloudQualitySettings(cloudQuality);
    }

    private static void SetLobbyTime()
    {
        uniStormSystem.CalculateHourAndMinute();
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormTimeRPC), RpcTarget.Others, new object[] { uniStormSystem.Hour, uniStormSystem.Minute, uniStormSystem.m_TimeFloat });
    }

    private static void SetLobbyWeather()
    {
        string name = uniStormSystem.CurrentWeatherType.WeatherTypeName;
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormWeatherRPC), RpcTarget.Others, new object[] { name });
    }

    public static void SetLobbyTimeWithCommand(int hour, int minute)
    {
        uniStormSystem.SetTime(hour, minute);
        uniStormSystem.CalculateTimeFloat();
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormTimeRPC), RpcTarget.Others, new object[] { uniStormSystem.Hour, uniStormSystem.Minute, uniStormSystem.m_TimeFloat });
    }

    public static void SetLobbyWeatherWithCommand(string name)
    {
        uniStormSystem.ChangeWeatherByName(name, true);
        RPCManager.PhotonView.RPC(nameof(RPCManager.SetUniStormWeatherRPC), RpcTarget.Others, new object[] { name });
    }
}
