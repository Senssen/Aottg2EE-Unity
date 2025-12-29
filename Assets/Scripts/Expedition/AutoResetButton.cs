using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoResetButton : MonoBehaviour
{
    private Button button;
    [SerializeField]
    public AudioSource sound;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        sound.Play();
        StartCoroutine(ResetNextFrame());
    }

    System.Collections.IEnumerator ResetNextFrame()
    {
        yield return null;
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
