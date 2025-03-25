using System.Collections;
using System.Collections.Generic;
using Funly.SkyStudio;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public SkyProfile _dayCycle;
    void Awake()
    {
        Invoke("TransitionSkybox",5);
    }
    private void TransitionSkybox()
    {
        TimeOfDayController.instance.StartSkyProfileTransition(_dayCycle, 20);
        Debug.Log("Called TransitionSkybox");
    }

}
