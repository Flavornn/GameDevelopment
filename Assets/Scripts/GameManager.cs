using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int roundNumber = 1;
    public Text roundText;
    public PlayerHealth player;
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    public Vector3 spawnPositions;
    public Text PingText;
    private bool Off = false;
    public GameObject DisconnectUI;

    [HideInInspector] public GameObject LocalPlayer;
    public Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float TimerAmount = 5f;
    private bool RunRespawnTimer = false;

    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public static int DeadPlayerActorNumber = -1;

    private void Awake()
    {
        _instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        CheckInput();

        if (PingText != null)
        {
            PingText.text = "Ping: " + PhotonNetwork.GetPing();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SpawnPlayer();
        }
        else if (scene.name == "PowerSelect")
        {
            Destroy(gameObject);
        }
    }


    public void CheckInput()
    {
        if (Off & Input.GetKeyDown(KeyCode.Escape))
        {
            DisconnectUI.SetActive(false);
            Off = false;
        }
        else if (!Off && Input.GetKeyDown(KeyCode.Escape))
        {
            DisconnectUI.SetActive(true);
            Off = true;
        }
    }

    public void SpawnPlayer()
    {
        if (PlayerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is not assigned!");
            return;
        }

        Vector3 spawnPos = new Vector3(
            Random.Range(-2.5f, 2.5f),
            spawnPositions.y,
            spawnPositions.z
        );

        GameObject playerObj = PhotonNetwork.Instantiate(
            PlayerPrefab.name,
            spawnPos,
            Quaternion.identity,
            0
        );

        if (playerObj != null)
        {
            PhotonView pv = playerObj.GetComponent<PhotonView>();
            if (pv.IsMine)
            {
                LocalPlayer = playerObj;
                Debug.Log("Spawned local player: " + LocalPlayer.name);

                if (GameCanvas != null)
                {
                    GameCanvas.SetActive(false);
                }
            }
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    private void HandlePlayerDeathRPC(int actorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DeadPlayer", actorNumber } });
            PhotonNetwork.LoadLevel("PowerSelect");
        }
    }

    public void HandlePlayerDeath(int actorNumber)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("HandlePlayerDeathRPC", RpcTarget.All, actorNumber);
    }
}