using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public PlayerHealth player;
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    private bool Off = false;
    public GameObject DisconnectUI;

    [HideInInspector] public GameObject LocalPlayer;
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;

    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public static int DeadPlayerActorNumber = -1;
    private float roundTime = 120f;
    private float roundTimer;
    private bool roundActive = false;

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

        // Round Timer
        if (roundActive)
        {
            roundTimer -= Time.deltaTime;

            if (roundTimer <= 0f)
            {
                roundActive = false;
                HandleTimeoutRound();
            }
        }
    }

    private void HandleTimeoutRound()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        if (players.Length < 2) return;

        var p1 = players[0];
        var p2 = players[1];

        float h1 = p1.GetCurrentHealth();
        float h2 = p2.GetCurrentHealth();

        if (h1 == h2) return; // Tie: do nothing

        PlayerHealth loser = h1 < h2 ? p1 : p2;
        HandlePlayerDeath(loser.photonView.Owner.ActorNumber);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            roundTimer = roundTime;
            roundActive = true;
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

        Transform spawnPoint = PhotonNetwork.IsMasterClient ? SpawnPoint1 : SpawnPoint2;

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned!");
            return;
        }

        GameObject playerObj = PhotonNetwork.Instantiate(
            PlayerPrefab.name,
            spawnPoint.position,
            spawnPoint.rotation,
            0
        );

        if (playerObj != null && playerObj.GetComponent<PhotonView>().IsMine)
        {
            LocalPlayer = playerObj;
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
        if (!PhotonNetwork.IsMasterClient) return;

        // Award round win to surviving player
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber != actorNumber)
            {
                int currentWins = p.CustomProperties.ContainsKey("Wins") ? (int)p.CustomProperties["Wins"] : 0;
                currentWins++;

                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "Wins", currentWins }
            };
                p.SetCustomProperties(props);

                // Check for victory
                if (currentWins >= 7)
                {
                    PhotonNetwork.LoadLevel("WinScene"); // You should create this
                    return;
                }
            }
        }

        // Move to Power Select scene
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DeadPlayer", actorNumber } });
        PhotonNetwork.LoadLevel("PowerSelect");
    }

    public float GetRemainingTime()
    {
        return roundTimer;
    }

    public void HandlePlayerDeath(int actorNumber)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("HandlePlayerDeathRPC", RpcTarget.All, actorNumber);
    }
}