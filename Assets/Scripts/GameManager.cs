using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int roundNumber = 1;
    public Text roundText;
    public PlayerHealth player;
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    public Vector3 spawnPositions;
    public Text PingText;
    private bool Off = false;
    public GameObject DisconnectUI;

    [HideInInspector]public GameObject LocalPlayer;
    public Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float TimerAmount = 5f;
    private bool RunRespawnTimer = false;

    private void Awake()
    {
        Instance = this;
        GameCanvas.SetActive(true);
    }

    private void Update()
    {
        CheckInput();
        PingText.text = "Ping: " + PhotonNetwork.GetPing();

        if(RunRespawnTimer)
        {
            StartRespawn();
        }
    }

    private void RespawnLocation()
    {
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
        RespawnTimerText.text = "Respawning in " + TimerAmount.ToString("F0");

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
        else if(!Off && Input.GetKeyDown(KeyCode.Escape))
        {
            DisconnectUI.SetActive(true);
            Off = true;
        }
    }

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-2.5f, 2.5f);
        GameObject player = PhotonNetwork.Instantiate(
        PlayerPrefab.name,
        spawnPositions,
        Quaternion.identity,
        0
    );
        if (player.GetComponent<PhotonView>().IsMine)
        {
            LocalPlayer = player;
        }

        GameCanvas.SetActive(false); 
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}