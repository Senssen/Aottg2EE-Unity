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
        if (gameObject.activeInHierarchy)
        {
            Vector3 mousePosition = CursorManager.GetInGameMousePosition();
            transform.position = new Vector3(mousePosition.x, mousePosition.y + 28f, mousePosition.z);
        }
    }
}

public enum EmbedState { Default, Crit }
