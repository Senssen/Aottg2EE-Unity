using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerButton PlayerButtonPrefab;
    [SerializeField]
    private Transform ScrollViewContent;
    private List<PlayerButton> PlayerListings = new List<PlayerButton>();

    [Header("Button Behavior")]
    private Button selectedButton;
    private Color selectedColor = new Color(0.12f, 0.23f, 0.34f);
    private Color neutralColor = new Color(0.15f, 0.30f, 0.46f);
    private Color hoverColor = new Color(0.30f, 0.52f, 0.77f);
    [SerializeField]
    AudioSource sound;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerButton listing = Instantiate(PlayerButtonPrefab, ScrollViewContent);
        if (listing != null)
            listing.SetPlayerInfo(newPlayer);
        PlayerListings.Add(listing);

        Button btn = listing.GetComponent<Button>();
        btn.onClick.AddListener(() => OnPlayerButtonClick(btn));

        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == otherPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnJoinedRoom()
    {
        PlayerButton listing = Instantiate(PlayerButtonPrefab, ScrollViewContent);
        if (listing != null)
            listing.SetPlayerInfo(PhotonNetwork.LocalPlayer);
        PlayerListings.Add(listing);

        Button btn = listing.GetComponent<Button>();
        btn.onClick.AddListener(() => OnPlayerButtonClick(btn));

        base.OnJoinedRoom();
    }


    public override void OnLeftRoom()
    {

        if (PlayerListings.Count > 0)
        {
            foreach (var item in PlayerListings)
            {
                Destroy(item.gameObject);
            }
            PlayerListings.Clear();
        }
        base.OnLeftRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        if (PlayerListings.Count > 0)
        {
            foreach (var item in PlayerListings)
            {
                Destroy(item.gameObject);
            }
            PlayerListings.Clear();
        }
        base.OnDisconnected(cause);
    }

    void OnPlayerButtonClick(Button btn)
    {
        if (selectedButton != null && selectedButton != btn)
        {
            SetButtonColor(selectedButton, neutralColor);
        }

        selectedButton = btn;
        SetButtonColor(btn, selectedColor);
        sound.Play();
    }

    void SetButtonColor(Button button, Color color)
    {
        var cb = button.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        button.colors = cb;
    }

    public void ResetSelectedButton()
    {
        var cb = selectedButton.colors;
        cb.normalColor = neutralColor;
        cb.selectedColor = selectedColor;
        cb.highlightedColor = hoverColor;
        cb.pressedColor = selectedColor;
        selectedButton.colors = cb;
        EmVariables.SelectedPlayer = null;
    }
}