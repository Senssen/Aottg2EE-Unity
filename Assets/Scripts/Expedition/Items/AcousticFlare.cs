using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using Photon.Realtime;
using Projectiles;
using Settings;
using UnityEngine;
using UnityEngine.UI;

public class AcousticFlare : MonoBehaviourPun
{
    [SerializeField] private Canvas Canvas;
    [SerializeField] public GameObject Marker; // the gameobject inside the Canvas that parents the UI elements

    [SerializeField] private RawImage BannerImage;

    [SerializeField] private Text OwnerText;

    [SerializeField] private Text DistanceText;

    [SerializeField] private AudioSource RingingSound;
    [SerializeField] private AudioSource FlareSound;

    private readonly float ViewportMinX = 0f;
    private readonly float ViewportMinY = 0f;
    private static readonly int MinRingRange = 250;

    private float _distance = 0f;
    private float _timeLeft = AcousticFlareController._maxLife;

    public void Setup(Player _player)
    {
        Color _color = GenerateRandomColor();
        if (photonView.IsMine)
            photonView.RPC(nameof(SetupRPC), RpcTarget.AllBuffered, new object[] { RemoveColorTagsFromName(_player.GetStringProperty(PlayerProperty.Name)), _color.r, _color.g, _color.b });
    }

    [PunRPC]
    private void SetupRPC(string _name, float _r, float _g, float _b, PhotonMessageInfo info)
    {
        OwnerText.text = _name;
        BannerImage.color = new Color(_r, _g, _b, .5f);

        if (PhotonNetwork.Time - info.SentServerTime < 0.5f)
        {
            FlareSound.Play();

            if ((int)Vector3.Distance(transform.position, SceneLoader.CurrentCamera.Camera.transform.position) < MinRingRange)
                RingingSound.Play();
        }

        UI.MinimapHandler.CreateWaypointMinimapIcon(transform);
    }

    private void ChangeCanvasLocation()
    {
        Vector3 viewportPosition = SceneLoader.CurrentCamera.Camera.WorldToViewportPoint(gameObject.transform.position);

        viewportPosition.x *= Canvas.pixelRect.width;
        viewportPosition.y *= Canvas.pixelRect.height;

        if (Vector3.Dot(gameObject.transform.position - SceneLoader.CurrentCamera.Camera.transform.position, SceneLoader.CurrentCamera.Camera.transform.forward) < 0)
        {
            if (viewportPosition.x < Screen.width / 2)
                viewportPosition.x = Screen.width - ViewportMinX;
            else
                viewportPosition.x = ViewportMinX;
        }

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, ViewportMinX, Screen.width - ViewportMinX);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, ViewportMinY, Screen.height - ViewportMinY);

        Marker.transform.position = viewportPosition;

        if (SceneLoader.CurrentCamera.Camera != null && DistanceText != null)
            DistanceText.text =  $"-{(int)_distance}U-";
    }

    private void ScaleUIElements()
    {
        if (_distance > 200f && _distance < 10000f)
        {
            float scale = 50f / _distance;
            Marker.transform.localScale = new Vector2(Mathf.Clamp(scale, 0.25f, .75f), Mathf.Clamp(scale, 0.25f, .75f));
        }
        else 
            Marker.transform.localScale = new Vector2(.25f, .25f);
    }

    private void ScaleOpacity()
    {
        if (_distance > 130f && _distance <= 10000f)
        {
            float scale = _distance / 1500f;
            ApplyOpacity(Mathf.Clamp(scale, .2f, .7f));
        }
        else
        {
            ApplyOpacity(0);
        }
    }

    private void ApplyOpacity(float opacity)
    {
        foreach (Transform child in Marker.transform)
        {
            RawImage _rawImage = child.gameObject.GetComponent<RawImage>();
            Text _text = child.gameObject.GetComponent<Text>();

            if (_text != null)
            {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, opacity);
            }
            if (_rawImage != null)
            {
                _rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, opacity);
            }
        }
    }

    private string RemoveColorTagsFromName(string playerName)
    {
        string pattern = @"<color=[^>]+>|</color>";
        string cleanName = System.Text.RegularExpressions.Regex.Replace(playerName, pattern, string.Empty);
        return cleanName;
    }

    private Color GenerateRandomColor()
    {
        float r = Random.Range(.01f, .99f);  
        float g = Random.Range(.01f, .99f);
        float b = Random.Range(.01f, .99f); 

        return new Color(r, g, b, .5f);
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        if (photonView.AmOwner)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
                DestroySelf();
        }

        _distance = Vector3.Distance(gameObject.transform.position, SceneLoader.CurrentCamera.Camera.transform.position);

        if (SceneLoader.CurrentCamera.Camera != null)
        {
            ChangeCanvasLocation();
            ScaleUIElements();
            ScaleOpacity();
        }
    }
}