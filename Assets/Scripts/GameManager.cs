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
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Remove PhotonView if not needed
        if (GetComponent<PhotonView>())
        {
            Destroy(GetComponent<PhotonView>());
    }
        //GameCanvas.SetActive(true);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            //SpawnPlayer();
        }
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

        if (RunRespawnTimer)
        {
            StartRespawn();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SpawnPlayer();
        }
    }

    private void RespawnLocation()
    {
        if (LocalPlayer == null) return;

        float RandomValue = Random.Range(-5, 5f);
        LocalPlayer.transform.localPosition = new Vector2(RandomValue, 3f);
    }

    private void StartRespawn()
    {
        if (LocalPlayer == null)
        {
            Debug.LogError("LocalPlayer is not assigned!");
            return;
        }
        TimerAmount -= Time.deltaTime;

        if (RespawnTimerText != null)
        {
            RespawnTimerText.text = "Respawning in " + TimerAmount.ToString("F0");
        }

        if (TimerAmount <= 0)
        {
            LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.All);
            LocalPlayer.GetComponent<PlayerHealth>().EnableInput();
            RespawnLocation();
            RespawnMenu.SetActive(false);
            RunRespawnTimer = false;
        }
    }

    public void EnableRespawn()
    {
        TimerAmount = 5f;
        RunRespawnTimer = true;
        RespawnMenu.SetActive(true);
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

    public void HandlePlayerDeath(int actorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DeadPlayer", actorNumber } });
            PhotonNetwork.LoadLevel("PowerSelect");
        }
    }
}