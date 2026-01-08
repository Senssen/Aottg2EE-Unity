using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using Photon.Realtime;
using Projectiles;
using UnityEngine;

public class AcousticFlare : MonoBehaviourPun
{
    public GameObject m_markerPrefab;
    public AcousticFlareMarker Marker; // the gameobject inside the Canvas that parents the UI elements
    public string OwnerName;
    public Color BannerColor;
    [SerializeField] private AudioSource RingingSound;
    [SerializeField] private AudioSource FlareSound;
    private static readonly int MinRingRange = 250;
    public float Distance = 0f;
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
        OwnerName = _name;
        BannerColor = new Color(_r, _g, _b, .5f);

        if (PhotonNetwork.Time - info.SentServerTime < 0.5f)
        {
            FlareSound.Play();
            if ((int)Vector3.Distance(transform.position, SceneLoader.CurrentCamera.Camera.transform.position) < MinRingRange)
                RingingSound.Play();
        }

        CreateHUDElement();
        UI.MinimapHandler.CreateWaypointMinimapIcon(transform);
    }

    private void CreateHUDElement()
    {
        GameObject expeditionUi = GameObject.Find("Expedition UI(Clone)");
        if (expeditionUi && expeditionUi.TryGetComponent(out ExpeditionUiManager expeditionUiManager) && expeditionUiManager.FlareMarkers)
        {
            GameObject go = Instantiate(m_markerPrefab, expeditionUiManager.FlareMarkers);
            Marker = go.GetComponent<AcousticFlareMarker>();
            Marker.Setup(this, OwnerName, BannerColor);
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
        Destroy(Marker.gameObject);
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

        Distance = (transform.position - SceneLoader.CurrentCamera.Camera.transform.position).magnitude;
        Marker.OnUpdate();
    }
}