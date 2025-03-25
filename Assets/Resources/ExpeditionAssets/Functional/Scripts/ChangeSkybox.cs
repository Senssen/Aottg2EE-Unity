using System.Collections;
using System.Collections.Generic;
using Funly.SkyStudio;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public SkyProfile _dayCycle;
    public TimeOfDayController _timeOfDayController;
    void Awake()
    {
        TransitionSkybox();
    }
    private void TransitionSkybox()
    {
        _timeOfDayController.StartSkyProfileTransition(_dayCycle, 2);
        Debug.Log("Called TransitionSkybox");
    }

}
