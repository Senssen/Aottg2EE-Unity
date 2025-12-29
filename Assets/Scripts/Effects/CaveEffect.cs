using ApplicationManagers;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CaveEffect : MonoBehaviour
{
    [SerializeField] private GameObject PostProcessingVolume;
    private PostProcessingManager _postProcessingManager;
    private BoxCollider _boxCollider;

    // --- Daylight disabling while inside cave ---
    private List<Light> _daylightLights = new();
    private Dictionary<Light, float> _daylightOriginalIntensity = new();
    private HashSet<Light> _daylightsDisabled = new();
    private bool _wasPlayerInCave;
    // ----------------------------------------------------------

    private void Start()
    {
        if (_postProcessingManager == null)
            _postProcessingManager = FindFirstObjectByType<PostProcessingManager>();

        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null)
        {
            Debug.LogWarning("Unsupported Object for Cave Effect (must have box collider).");
            this.enabled = false;
            return;
        }

        // Cache all directional lights at startup
        // If directional lights are added/removed at runtime, they won’t be handled
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var light in allLights)
        {
            if (light != null && light.type == LightType.Directional)
            {
                _daylightLights.Add(light);
            }
        }
    }

    private static bool IsInsideBounds(Vector3 worldPos, BoxCollider bc)
    {
        Vector3 localPos = bc.transform.InverseTransformPoint(worldPos);
        Vector3 delta = localPos - bc.center + bc.size * 0.5f;
        return Vector3.Max(Vector3.zero, delta) == Vector3.Min(delta, bc.size);
    }

    private void Update()
    {
        bool playerInCave = IsInsideBounds(SceneLoader.CurrentCamera.Camera.transform.position, _boxCollider);

        if (playerInCave && !_wasPlayerInCave)
        {
            // Player just entered the cave
            ApplyCaveEffect();
        }
        else if (!playerInCave && _wasPlayerInCave)
        {
            // Player just exited the cave
            DisableCaveEffect();
        }
    }

    private void ApplyCaveEffect()
    {
        if (PostProcessingVolume != null) PostProcessingVolume.SetActive(true);
        if (_postProcessingManager != null) _postProcessingManager.SetState(false);
        DisableDaylight();
        _wasPlayerInCave = true;
    }

    private void DisableCaveEffect()
    {
        if (PostProcessingVolume != null) PostProcessingVolume.SetActive(false);
        if (_postProcessingManager != null) _postProcessingManager.SetState(true);
        RestoreDaylight();
        _wasPlayerInCave = false;
    }

    private void DisableDaylight()
    {
        foreach (var light in _daylightLights)
        {
            if (light != null && light.enabled && !_daylightsDisabled.Contains(light))
            {
                _daylightOriginalIntensity[light] = light.intensity;
                light.enabled = false;
                _daylightsDisabled.Add(light);
            }
        }
    }

    private void RestoreDaylight()
    {
        foreach (var light in _daylightsDisabled)
        {
            if (light != null)
            {
                light.enabled = true;
                if (_daylightOriginalIntensity.TryGetValue(light, out float orig))
                {
                    light.intensity = orig;
                }
            }
        }
        _daylightsDisabled.Clear();
        _daylightOriginalIntensity.Clear();
    }
}