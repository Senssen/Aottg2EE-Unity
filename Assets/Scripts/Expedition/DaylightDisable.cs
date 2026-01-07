using GameManagers;
using Photon.Pun;
using Settings;
using UnityEngine;

public class DaylightDisable : MonoBehaviour
{
    // This script serves the sole purpose of disabling Unity daylight components added by the Custom Logic of AoTTG2 maps.
    // It is required to do this directly on the component because there are timing issues on the event where the scene gets loaded
    // but the daylight components are not yet initialized.
    // Because of this, we need to ensure that the daylight component is disabled on its own game object when it awakens, without any other dependency.

    // (Fuck custom logic.)
    public void HandleDisable()
    {
        if (DynamicWeatherManager.IsEnabled())
            gameObject.SetActive(false);
    }
}
