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
            GameObject marker = PhotonNetwork.Instantiate(ResourcePaths.UI + "/Expedition/AcousticFlareMarker", position, rotation, 0);
            AcousticFlare acousticFlare = marker.GetComponent<AcousticFlare>();
            acousticFlare.Setup(marker.transform, PhotonNetwork.LocalPlayer);
            PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/AcousticParticle", position, rotation, 0);
        }

        public static void ChangeAcousticFlaresVisibility(bool visible)
        {
            AcousticFlare[] acousticFlares = GameObject.FindObjectsByType<AcousticFlare>(FindObjectsSortMode.None);
            for (int i = 0; i < acousticFlares.Length; i++)
                acousticFlares[i].marker.SetActive(visible);
        }
    }
}