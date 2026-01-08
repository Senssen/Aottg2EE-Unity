using ApplicationManagers;
using UnityEngine;
using UnityEngine.UI;

public class AcousticFlareMarker : MonoBehaviour
{
    private AcousticFlare AcousticFlare;
    private Canvas HUDCanvas;
    [SerializeField] private RawImage BannerImage;
    [SerializeField] private Text OwnerText;
    [SerializeField] private Text DistanceText;
    private readonly float ViewportMinX = 0f;
    private readonly float ViewportMinY = 0f;
    private static readonly int MaxRenderDistance = 10000;
    private static readonly float MinMarkerScale = .125f;
    private static readonly float MaxMarkerScale = .5f;

    void Awake()
    {
        ApplyOpacity(0);
    }

    public void Setup(AcousticFlare _acousticFlare, string _ownerName, Color _bannerColor)
    {
        HUDCanvas = GetComponentInParent<Canvas>();
        AcousticFlare = _acousticFlare;
        OwnerText.text = _ownerName;
        BannerImage.color = _bannerColor;
    }

    private void ChangeCanvasLocation()
    {
        Vector3 flarePosition = AcousticFlare.transform.position;
        Vector3 viewportPosition = SceneLoader.CurrentCamera.Camera.WorldToViewportPoint(flarePosition);

        viewportPosition.x *= HUDCanvas.pixelRect.width;
        viewportPosition.y *= HUDCanvas.pixelRect.height;

        if (Vector3.Dot(flarePosition - SceneLoader.CurrentCamera.Camera.transform.position, SceneLoader.CurrentCamera.Camera.transform.forward) < 0)
        {
            if (viewportPosition.x < Screen.width / 2)
                viewportPosition.x = Screen.width - ViewportMinX;
            else
                viewportPosition.x = ViewportMinX;
        }

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, ViewportMinX, Screen.width - ViewportMinX);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, ViewportMinY, Screen.height - ViewportMinY);

        transform.position = viewportPosition;

        if (SceneLoader.CurrentCamera.Camera != null && DistanceText != null)
            DistanceText.text = $"-{(int)AcousticFlare.Distance}U-";
    }

    private void ScaleSize()
    {
        if (AcousticFlare.Distance > 200f && AcousticFlare.Distance < MaxRenderDistance)
        {
            float scale = 50f / AcousticFlare.Distance;
            transform.localScale = new Vector2(Mathf.Clamp(scale, MinMarkerScale, MaxMarkerScale), Mathf.Clamp(scale, MinMarkerScale, MaxMarkerScale));
        }
        else 
            transform.localScale = new Vector2(MinMarkerScale, MinMarkerScale);
    }

    private void ScaleOpacity()
    {
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
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Text _text))
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, opacity);
            if (child.TryGetComponent(out RawImage _rawImage))
                _rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, opacity);
        }
    }

    public void OnUpdate()
    {
        if (AcousticFlare == null)
            return;

        else if (SceneLoader.CurrentCamera.Camera != null)
        {
            ChangeCanvasLocation();
            ScaleSize();
            ScaleOpacity();
        }
    }
}