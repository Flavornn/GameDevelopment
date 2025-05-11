using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject WaitingPanel;
    [SerializeField] private TMP_Text WaitingText;
    [SerializeField] private InputField UsernameInput;
    [SerializeField] private GameObject StartButton;

    private bool isSearching = false;
    private const string MatchmakingRoomName = "MatchmakingRoom";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = VersionName;
    }

    private void Start()
    {
        UsernameMenu.SetActive(true);
        WaitingPanel.SetActive(false);
    }

    public void ChangeUsernameInput()
    {
        StartButton.SetActive(UsernameInput.text.Length >= 1);
    }

    public void StartMatchmaking()
    {
        if (isSearching) return;

        PhotonNetwork.NickName = UsernameInput.text;
        UsernameMenu.SetActive(false);
        WaitingPanel.SetActive(true);
        WaitingText.text = "Searching for opponent...";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            isSearching = true;
        }
        else
        {
            JoinMatchmaking();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isSearching)
        {
            JoinMatchmaking();
        }
    }

    private void JoinMatchmaking()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinRandomOrCreateRoom(
            roomOptions: roomOptions,
            expectedMaxPlayers: 2
        );
    }

    public override void OnJoinedRoom()
    {
        WaitingText.text = $"Waiting for opponent... ({PhotonNetwork.CurrentRoom.PlayerCount}/2)";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(StartGame());
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        WaitingText.text = "Opponent found! Starting game...";
        yield return new WaitForSeconds(1.5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public void CancelMatchmaking()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        isSearching = false;
        WaitingPanel.SetActive(false);
        UsernameMenu.SetActive(true);
    }
}