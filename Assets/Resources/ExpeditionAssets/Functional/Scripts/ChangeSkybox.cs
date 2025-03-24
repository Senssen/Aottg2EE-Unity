using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    private GameObject _gameObject;
    private Skybox _skybox;
    public Material _skyboxMaterial;
    void Awake()
    {
        Invoke("UpdateSkybox",5);
    }

    private void UpdateSkybox()
    {
        _gameObject = GameObject.Find("MainCamera(Clone)");
        _skybox = _gameObject.GetComponent<Skybox>();
        _skybox.material = _skyboxMaterial;

    }

}
