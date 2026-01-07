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
    [SerializeField] private Canvas canvas;
    [SerializeField] public GameObject marker; // the gameobject inside the canvas that parents the UI elements

    [SerializeField] private RawImage bannerImage;

    [SerializeField] private Text ownerName;

    [SerializeField] private Text distance;

    [SerializeField] private AudioSource ringingSound;
    [SerializeField] private AudioSource flareSound;

    private Transform uiTransform;

    private float minX;
    private float minY;
    private float timeLeft = AcousticFlareController._maxLife;
    private static readonly int minRingingRange = 250;

    public void Setup(Player _player)
    {
        Color _color = GenerateRandomColor();
        if (photonView.IsMine)
            photonView.RPC(nameof(SetupRPC), RpcTarget.AllBuffered, new object[] { RemoveColorTagsFromName(_player.GetStringProperty(PlayerProperty.Name)), _color.r, _color.g, _color.b });
    }

    [PunRPC]
    private void SetupRPC(string _name, float _r, float _g, float _b, PhotonMessageInfo info)
    {
        minX = 0;
        minY = 0;
        ownerName.text = _name;
        bannerImage.color = new Color(_r, _g, _b, .5f);

        if (PhotonNetwork.Time - info.SentServerTime < 0.5f)
        {
            flareSound.Play();

            if ((int)Vector3.Distance(transform.position, SceneLoader.CurrentCamera.Camera.transform.position) < minRingingRange)
                ringingSound.Play();
        }

        UI.MinimapHandler.CreateWaypointMinimapIcon(transform);
    }

    private void ChangeCanvasLocation()
    {
        Vector3 pos = SceneLoader.CurrentCamera.Camera.WorldToViewportPoint(gameObject.transform.position);

        pos.x *= canvas.pixelRect.width;
        pos.y *= canvas.pixelRect.height;

        if (Vector3.Dot(gameObject.transform.position - SceneLoader.CurrentCamera.Camera.transform.position, SceneLoader.CurrentCamera.Camera.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
                pos.x = Screen.width - minX;
            else
                pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, Screen.width - minX);
        pos.y = Mathf.Clamp(pos.y, minY, Screen.height - minY);

        marker.transform.position = pos;

        if (SceneLoader.CurrentCamera.Camera != null && distance != null)
            distance.text = "-" + ((int)Vector3.Distance(gameObject.transform.position, SceneLoader.CurrentCamera.Camera.transform.position)).ToString() + "U-";
    }

    private void ScaleUIElements()
    {
        float _distanceValue = Vector3.Distance(gameObject.transform.position, SceneLoader.CurrentCamera.Camera.transform.position);

        if (_distanceValue > 200f && _distanceValue < 10000f)
        {
            float scale = 50f / _distanceValue;
            marker.transform.localScale = new Vector2(Mathf.Clamp(scale, 0.25f, .75f), Mathf.Clamp(scale, 0.25f, .75f));
        }
        else 
            marker.transform.localScale = new Vector2(.25f, .25f);
    }

    private void ScaleOpacity()
    {
        float _distanceValue = Vector3.Distance(gameObject.transform.position, SceneLoader.CurrentCamera.Camera.transform.position);
        if (_distanceValue > 130f && _distanceValue <= 10000f)
        {
            float scale = _distanceValue / 1500f;
            ApplyOpacity(Mathf.Clamp(scale, .2f, .7f));
        }
        else
        {
            ApplyOpacity(0);
        }
    }

    private void ApplyOpacity(float opacity)
    {
        foreach (Transform child in marker.transform)
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
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
                DestroySelf();
        }

        if (SceneLoader.CurrentCamera.Camera != null)
        {
            ChangeCanvasLocation();
            ScaleUIElements();
            ScaleOpacity();
        }
    }
}