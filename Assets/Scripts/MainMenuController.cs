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
    private const string GAME_MODE = "DuelMode"; // Used to filter rooms by game mode

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = VersionName;
        
        // Configure room options for all rooms
        PhotonNetwork.SerializationRate = 30; // Default is 30
        PhotonNetwork.SendRate = 30; // Default is 30
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
        WaitingText.text = "Connecting to server...";

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
        Debug.Log("Connected to Master Server");
        if (isSearching)
        {
            JoinMatchmaking();
        }
    }

    private void JoinMatchmaking()
    {
        WaitingText.text = "Searching for match...";

        // Set up the room options
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true, // Needed to identify players
            CleanupCacheOnLeave = true // This will clean up instantiated objects when a player leaves
        };

        // Add custom room properties
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "GameMode", GAME_MODE }
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "GameMode" };

        // Try to join a random room with our game mode
        PhotonNetwork.JoinRandomRoom(
            new ExitGames.Client.Photon.Hashtable { { "GameMode", GAME_MODE } },
            2
        );
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room. Creating new room...");
        WaitingText.text = "Creating new room...";

        // Create a new room with a unique name
        string roomName = "Room_" + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            CleanupCacheOnLeave = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "GameMode", GAME_MODE } },
            CustomRoomPropertiesForLobby = new string[] { "GameMode" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name} Players: {PhotonNetwork.CurrentRoom.PlayerCount}/2");
        WaitingText.text = $"Waiting for opponent... ({PhotonNetwork.CurrentRoom.PlayerCount}/2)";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(StartGame());
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined. Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
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
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        isSearching = false;
        WaitingPanel.SetActive(false);
        UsernameMenu.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from server: {cause}");
        isSearching = false;
        WaitingPanel.SetActive(false);
        UsernameMenu.SetActive(true);
    }
}