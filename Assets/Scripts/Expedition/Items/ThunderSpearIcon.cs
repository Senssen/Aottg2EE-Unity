using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class ThunderSpearIcon : MonoBehaviour
{
    [SerializeField] Color DefaultColor;
    [SerializeField] Color CritColor;
    private RawImage Icon;
    void Awake()
    {
        Icon = GetComponent<RawImage>();
        gameObject.SetActive(false);
    }

    public void Activate(bool active)
    {
        gameObject.SetActive(active);

        if (active == false)
            SetColor(EmbedState.Default);
    }

    public void SetColor(EmbedState state)
    {
        switch (state)
        {
            case EmbedState.Default:
                Icon.color = DefaultColor;
                break;
            default:
                Icon.color = CritColor;
                break;
        }
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy)
        return;

        RectTransform crosshair = CursorManager.GetCachedCrosshairTransform();
        RectTransform self = (RectTransform)transform;

        float scaledOffsetY = (25f * crosshair.lossyScale.y) + 5f;
        self.position = crosshair.position + new Vector3(0f, scaledOffsetY, 0f);
    }
}

public enum EmbedState { Default, Crit }
