using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject JoinRoomPanel;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject WaitingPanel;

    [Header("Username Menu")]
    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private Button ConfirmButton;

    [Header("Main Menu Panel")]
    [SerializeField] private TMP_Text WelcomeText;

    [Header("Join Room Panel")]
    [SerializeField] private TMP_InputField JoinCodeInput;
    [SerializeField] private TMP_Text JoinErrorText;
    [SerializeField] private Button JoinButton;

    [Header("Lobby Panel")]
    [SerializeField] private TMP_Text RoomCodeText;
    [SerializeField] private TMP_Text Player1Text;
    [SerializeField] private TMP_Text Player2Text;
    [SerializeField] private TMP_Text CountdownText;

    [Header("Waiting Panel")]
    [SerializeField] private TMP_Text WaitingText;

    private const string GAME_MODE = "DuelMode";
    private bool isQuickplay = false;
    private bool isCreatingRoom = false;
    private bool isJoiningRoom = false;
    private string pendingRoomCode;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.1";
    }

    private void Start()
    {
        UsernameMenu.SetActive(true);
        MainMenuPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        WaitingPanel.SetActive(false);

        JoinCodeInput.characterLimit = 5;
        JoinCodeInput.contentType = TMP_InputField.ContentType.Alphanumeric;
        JoinCodeInput.onValueChanged.AddListener(ValidateRoomCode);
        PhotonNetwork.SendRate = 30;             
        PhotonNetwork.SerializationRate = 20;
    }

    public void ValidateUsername() => ConfirmButton.interactable = UsernameInput.text.Length >= 1;

    public void ConfirmUsername()
    {
        PhotonNetwork.NickName = UsernameInput.text;
        UsernameMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
        WelcomeText.text = $"Welcome {UsernameInput.text}!";
    }

    public void Quickplay()
    {
        isQuickplay = true;
        MainMenuPanel.SetActive(false);
        WaitingPanel.SetActive(true);
        WaitingText.text = "Connecting...";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            StartMatchmaking();
        }
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            isCreatingRoom = true;
            MainMenuPanel.SetActive(false);
            WaitingPanel.SetActive(true);
            WaitingText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            CreateRoomInternal();
        }
    }

    private void CreateRoomInternal()
    {
        string roomCode = Random.Range(10000, 100000).ToString();
        var props = new ExitGames.Client.Photon.Hashtable { { "GameMode", GAME_MODE } };
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            CustomRoomProperties = props,
            CustomRoomPropertiesForLobby = new string[] { "GameMode" }
        };

        PhotonNetwork.CreateRoom(roomCode, options);
        WaitingPanel.SetActive(false);
    }

    public void ShowJoinPanel()
    {
        MainMenuPanel.SetActive(false);
        JoinRoomPanel.SetActive(true);
        JoinErrorText.gameObject.SetActive(false);
    }

    private void ValidateRoomCode(string code)
    {
        JoinButton.interactable = code.Length == 5;
        JoinErrorText.gameObject.SetActive(false);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(JoinCodeInput.text))
        {
            JoinErrorText.text = "Enter a room code!";
            JoinErrorText.gameObject.SetActive(true);
            return;
        }

        if (!PhotonNetwork.IsConnected)
        {
            isJoiningRoom = true;
            pendingRoomCode = JoinCodeInput.text;
            JoinRoomPanel.SetActive(false);
            WaitingPanel.SetActive(true);
            WaitingText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            AttemptRoomJoin(JoinCodeInput.text);
        }
    }

    private void AttemptRoomJoin(string roomCode)
    {
        WaitingText.text = "Joining room...";
        PhotonNetwork.JoinRoom(roomCode);
    }

    public override void OnConnectedToMaster()
    {
        if (isQuickplay)
        {
            isQuickplay = false;
            StartMatchmaking();
        }
        else if (isCreatingRoom)
        {
            isCreatingRoom = false;
            CreateRoomInternal();
        }
        else if (isJoiningRoom)
        {
            isJoiningRoom = false;
            AttemptRoomJoin(pendingRoomCode);
        }
    }

    private void StartMatchmaking()
    {
        WaitingPanel.SetActive(true);
        WaitingText.text = "Searching for match...";
        PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable { { "GameMode", GAME_MODE } }, 2);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // No available rooms -> create one
        CreateRoomInternal();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        JoinErrorText.text = "Failed to join room!\n• Check code is correct\n• Room might be full";
        JoinErrorText.gameObject.SetActive(true);
        JoinRoomPanel.SetActive(true);
        WaitingPanel.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        WaitingPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        RoomCodeText.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    private void UpdatePlayerList()
    {
        var players = PhotonNetwork.PlayerList;
        Player1Text.text = players.Length > 0 ? players[0].NickName : "...";
        Player2Text.text = players.Length > 1 ? players[1].NickName : "...";
    }

    private IEnumerator StartCountdown()
    {
        CountdownText.gameObject.SetActive(true);
        int timer = 5;
        while (timer > 0)
        {
            CountdownText.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }
        CountdownText.text = "0";
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    public void CopyRoomCode() => GUIUtility.systemCopyBuffer = RoomCodeText.text;

    public void OnBackButton()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        PhotonNetwork.Disconnect();
        JoinRoomPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        WaitingPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void QuitGame() => Application.Quit();
}
