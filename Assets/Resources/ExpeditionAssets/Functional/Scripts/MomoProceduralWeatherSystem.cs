using System.Collections;
using System.Collections.Generic;
using Funly.SkyStudio;
using UnityEditor.VersionControl;
using UnityEngine;

public class MomoProceduralWeatherSystem : MonoBehaviour
{
    public TimeOfDayController _timeOfDayController;
    public SkyProfile _momoDayCycle;
    public SkyProfile _stormy;
    private float stormRollTimer = 0f;
    private float dayCycleTimer = 0f;
    private bool calledStormTimer = false;
    private bool calledDayCycleTimer = true;


    private void Update()
    {
        RollStormy();
        TriggerDayCycle();
    }

    private void RollStormy()
    {
        if (!calledStormTimer)
            stormRollTimer += Time.deltaTime;

        if (stormRollTimer >= 60f)
        {
            calledStormTimer = true;
            stormRollTimer = 0f;
            int rngCheck = Random.Range(1, 4);
            if (rngCheck == 1)
            {
                TriggerStormy();
            }
            else
            {
                calledStormTimer = false;
            }
        }

    }

    private void TriggerStormy()
    {
        _timeOfDayController.skyProfile = _stormy;
        calledDayCycleTimer = false;

    }

    private void TriggerDayCycle()
    {
        if (!calledDayCycleTimer)
        dayCycleTimer += Time.deltaTime;

        if (dayCycleTimer >= 90f)
        {
            calledDayCycleTimer = true;
            calledStormTimer = false;
            _timeOfDayController.skyProfile = _momoDayCycle;
            dayCycleTimer = 0f;
        }

    }
}
