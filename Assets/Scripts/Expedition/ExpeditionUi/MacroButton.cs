using UnityEngine;
using TMPro;
using System.IO;
using GameManagers;
using System.Collections;
using System.Globalization;

public class MacroButton : MonoBehaviour
{
    [SerializeField] private TMP_Text Text;
    private MacrosManager macrosManager;
    private string FullPath;

    public void Initialize(string name, string path)
    {
        FullPath = path;
        Text.text = name;
        GameObject expeditionUi = GameObject.Find("Expedition UI(Clone)");
        macrosManager = expeditionUi.GetComponent<MacrosManager>();
        if (expeditionUi && expeditionUi.TryGetComponent(out AudioSource audioSource) && TryGetComponent(out AutoResetButton autoResetButton))
            autoResetButton.sound = audioSource;

    }

    public void HandleClick()
    {
        if (!File.Exists(FullPath))
        {
            ChatManager.AddLine($"Macro file not found: {FullPath}", ChatTextColor.Error);
            return;
        }

        macrosManager.ExecuteFile(FullPath);
    }
}