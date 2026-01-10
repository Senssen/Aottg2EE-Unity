using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CarpenterUI : MonoBehaviour
{
    [SerializeField] List<Placeable> placeables;
    [SerializeField] GameObject buildablesCanvas;
    
    void Start()
    {
        foreach (Placeable placeable in placeables)
        {
            GameObject go = new GameObject(placeable.displayName);
            go.transform.SetParent(buildablesCanvas.transform);
            go.AddComponent<RectTransform>();
            Button button = go.AddComponent<Button>();
            Image image = go.AddComponent<Image>();
            image.sprite = placeable.icon;
        }
    }

    void Update()
    {
    }
}
