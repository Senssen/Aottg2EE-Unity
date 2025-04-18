using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmVariables
{
    public static bool IsOpen = false;
    public static Player SelectedPlayer;
    public static int LogisticianMaxSupply = 4;

    public static void SetActive(bool b)
    {
        IsOpen = b;
    }
}