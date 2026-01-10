using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Expedition/Placeable")]
public class Placeable : ScriptableObject
{
    public string displayName;
    public GameObject prefab;
    public Sprite icon;
}
