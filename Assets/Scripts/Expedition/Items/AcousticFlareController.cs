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
        public static void FireAcousticFlare(Vector3 position, Quaternion rotation)
        {
            GameObject marker = PhotonNetwork.Instantiate(ResourcePaths.UI + "/Expedition/AcousticFlareMarker", position, rotation, 0);
            AcousticFlare _settings = marker.GetComponent<AcousticFlare>();
            _settings.Setup(marker.transform, PhotonNetwork.LocalPlayer);
            PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/AcousticParticle", position, rotation, 0);
        }
    }
}