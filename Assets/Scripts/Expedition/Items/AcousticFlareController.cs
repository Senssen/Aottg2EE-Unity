using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using Settings;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utility;

namespace Projectiles
{
    static class AcousticFlareController
    {
        public static readonly float _maxLife = 180f;

        public static void FireAcousticFlare(Vector3 position, Quaternion rotation)
        {
            GameObject go = PhotonNetwork.Instantiate(ResourcePaths.UI + "/Expedition/AcousticFlare", position, rotation, 0);
            AcousticFlare acousticFlare = go.GetComponent<AcousticFlare>();
            acousticFlare.Setup(PhotonNetwork.LocalPlayer);
        }

        public static void ChangeAcousticFlaresVisibility(bool visible)
        {
            AcousticFlareMarker[] acousticFlareMarkerss = GameObject.FindObjectsByType<AcousticFlareMarker>(FindObjectsSortMode.None);
            for (int i = 0; i < acousticFlareMarkerss.Length; i++)
                acousticFlareMarkerss[i].gameObject.SetActive(visible);
        }
    }
}