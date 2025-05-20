using GameManagers;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class PlayerButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Button_Text;
    public Player PhotonPlayer { get; private set; }

    public void SetPlayerInfo(Player player)
    {
        PhotonPlayer = player;
        NameRefresh();
    }
    public void OnClick_Button()
    {
        EmVariables.SelectedPlayer = PhotonPlayer;
    }

    public void NameRefresh()
    {
        Button_Text.text = $"[{PhotonPlayer.ActorNumber}]" + $" {SanitizeText(PhotonPlayer.GetStringProperty(PlayerProperty.Name))}";
    }

    private string SanitizeText(string text)
    {
        return Regex.Replace(text, "<.*?>", string.Empty);
    }
}