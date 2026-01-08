using System;
using ApplicationManagers;
using UnityEngine;
using UnityEngine.UI;

public class AcousticFlareMarker : MonoBehaviour
{
    private AcousticFlare AcousticFlare;
    private Canvas HUDCanvas;
    [SerializeField] private RawImage MarkerImage;
    [SerializeField] private RawImage BannerImage;
    [SerializeField] private Text OwnerText;
    [SerializeField] private Text DistanceText;
    private readonly float ViewportMinX = 0f;
    private readonly float ViewportMinY = 0f;
    private static readonly int MaxRenderDistance = 10000;
    private static readonly float MinMarkerScale = .125f;
    private static readonly float MaxMarkerScale = .5f;
    private static readonly float InitializeTime = 2f;
    private float _currentTime;
    private bool _initialized = false;

    public void Setup(AcousticFlare _acousticFlare, string _ownerName, Color _bannerColor)
    {
        _currentTime = InitializeTime;
        HUDCanvas = GetComponentInParent<Canvas>();
        AcousticFlare = _acousticFlare;
        OwnerText.text = _ownerName;
        BannerImage.color = _bannerColor;
        ApplyOpacity(0);
    }

    private void ChangeCanvasLocation()
    {
        Vector3 viewportPosition = SceneLoader.CurrentCamera.Camera.WorldToViewportPoint(AcousticFlare.transform.position);

        viewportPosition.x *= HUDCanvas.pixelRect.width;
        viewportPosition.y *= HUDCanvas.pixelRect.height;

        if (Vector3.Dot(AcousticFlare.transform.position - SceneLoader.CurrentCamera.Camera.transform.position, SceneLoader.CurrentCamera.Camera.transform.forward) < 0)
        {
            if (viewportPosition.x < Screen.width / 2)
                viewportPosition.x = Screen.width - ViewportMinX;
            else
                viewportPosition.x = ViewportMinX;
        }

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, ViewportMinX, Screen.width - ViewportMinX);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, ViewportMinY, Screen.height - ViewportMinY);

        transform.position = new Vector3(viewportPosition.x, viewportPosition.y, 0);

        if (SceneLoader.CurrentCamera.Camera != null && DistanceText != null)
            DistanceText.text = $"-{(int)AcousticFlare.Distance}U-";
    }

    private void ScaleSize()
    {
        Vector2 targetScale;
        if (AcousticFlare.Distance > 200f && AcousticFlare.Distance < MaxRenderDistance)
        {
            float scale = Math.Clamp(50f / AcousticFlare.Distance, MinMarkerScale, MaxMarkerScale);
            targetScale = new Vector2(scale, scale);
        }
        else
        {
            targetScale = new Vector2(MinMarkerScale, MinMarkerScale);
        }

        transform.localScale = Vector2.Lerp(transform.localScale, targetScale, Time.deltaTime * 2.5f);
    }

    private void ScaleOpacity()
    {
        if (!_initialized)
            return;

        if (AcousticFlare.Distance > 130f && AcousticFlare.Distance <= MaxRenderDistance)
        {
            float scale = AcousticFlare.Distance / 1500f;
            ApplyOpacity(Mathf.Clamp(scale, .2f, .7f));
        }
        else
        {
            ApplyOpacity(0);
        }
    }

    private void ApplyOpacity(float opacity)
    {
        MarkerImage.color = new Color(MarkerImage.color.r, MarkerImage.color.g, MarkerImage.color.b, opacity);
        BannerImage.color = new Color(BannerImage.color.r, BannerImage.color.g, BannerImage.color.b, opacity);
        OwnerText.color = new Color(OwnerText.color.r, OwnerText.color.g, OwnerText.color.b, opacity);
        DistanceText.color = new Color(DistanceText.color.r, DistanceText.color.g, DistanceText.color.b, opacity);
    }

    public void OnUpdate()
    {
        if (AcousticFlare == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_currentTime > 0)
            _currentTime -= Time.deltaTime;
        else
            _initialized = true;

        if (SceneLoader.CurrentCamera.Camera != null)
        {
            ChangeCanvasLocation();
            ScaleSize();
            ScaleOpacity();
        }
    }
}