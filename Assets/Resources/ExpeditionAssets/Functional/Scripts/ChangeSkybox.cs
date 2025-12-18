using System.Collections;
using System.Collections.Generic;
using Funly.SkyStudio;
using UniStorm.Effects;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    void Awake()
    {
        Invoke("DisableSkybox", 3);
    }

    private void DisableSkybox()
    {
        GameObject mainCamera = GameObject.Find("MainCamera(Clone)");
        Skybox skybox = mainCamera.GetComponent<Skybox>();
        skybox.enabled = false;
    }

}
