using UnityEngine;
using System.IO;
using Utility;
using Settings;
using UI;
using GameManagers;
using CustomLogic;
using System.Globalization;
using System.Collections;

public class MacrosManager : MonoBehaviour
{
    private readonly string FolderPath = FolderPaths.Macros;
    [SerializeField] private Transform VerticalGroup;
    [SerializeField] private MacroButton MacroButton;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private RectTransform AnimatedBackground;
    private readonly float AnimationDuration = 0.25f;
    private readonly Vector3 FULL_SCALE = new Vector3(0.3f, 0.95f, 0f);
    private HumanInputSettings _humanInput;
    void Start()
    {
        _humanInput = SettingsManager.InputSettings.Human;
    }

    void Update()
    {
        if (_humanInput.MacrosMenu.GetKeyDown() && !InGameMenu.InMenu() && !ChatManager.IsChatActive() && !CustomLogicManager.Cutscene)
            SetActive(true);
        else if (_humanInput.MacrosMenu.GetKeyDown() && Canvas.GetActive() == true)
            SetActive(false);
    }

    private void SetMacros()
    {
        DirectoryInfo directoryInfo = Directory.CreateDirectory(FolderPath);
        FileInfo[] files = directoryInfo.GetFiles("*.txt");
        for (int i = files.Length - 1; i >= 0; i--)
        {
            FileInfo file = files[i];
            string displayName = Path.GetFileNameWithoutExtension(file.Name);
            MacroButton button = Instantiate(MacroButton, VerticalGroup);
            button.Initialize(displayName, file.FullName);
        }
    }

    private void ClearMacros()
    {
        for (int i = VerticalGroup.childCount - 1; i >= 0; i--)
            Destroy(VerticalGroup.GetChild(i).gameObject);
    }

    public void SetActive(bool open)
    {
        if (open == true)
        {
            SetMacros();
            Canvas.SetActive(true);
            EmVariables.IsOpen = true;

            StartCoroutine(AnimateUI(true));
        }
        else
        {
            StartCoroutine(AnimateUI(false));
        }
    }

    System.Collections.IEnumerator AnimateUI(bool isOpening)
    {
        float time = 0f;
        while (time < AnimationDuration)
        {
            if (isOpening)
            {
                AnimatedBackground.localScale = Vector3.Lerp(Vector3.zero, FULL_SCALE, time / AnimationDuration);
            }
            else
            {
                AnimatedBackground.localScale = Vector3.Lerp(FULL_SCALE, Vector3.zero, time / AnimationDuration);
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (isOpening)
        {
            AnimatedBackground.localScale = FULL_SCALE;
        }
        else
        {
            ClearMacros();
            AnimatedBackground.localScale = Vector3.zero;
            Canvas.SetActive(false);
            EmVariables.IsOpen = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SetActive(false);
        }
    }

    public void ExecuteFile(string path)
    {
        StartCoroutine(RunMacro(path));
    }

    private IEnumerator RunMacro(string path)
    {
        foreach (string rawLine in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            string line = rawLine.Trim();
            if (line.StartsWith("WAIT", System.StringComparison.OrdinalIgnoreCase))
            {
                float seconds = ParseWait(line);
                if (seconds > 0f)
                    yield return new WaitForSeconds(seconds);

                continue;
            }

            ChatManager.HandleInput(rawLine);
        }
    }

    private float ParseWait(string line)
    {
        string[] parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
            return 1f;

        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float seconds))
        {
            ChatManager.AddLine($"Invalid WAIT value: '{line}'", ChatTextColor.Error);
            return -1f;
        }

        if (seconds <= 0f)
        {
            ChatManager.AddLine($"WAIT must be > 0: '{line}'", ChatTextColor.Error);
            return -1f;
        }

        return seconds;
    }
}
