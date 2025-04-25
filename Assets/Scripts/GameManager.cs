using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
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
    public string gameOverSceneName = "PowerSelect";


    [HideInInspector]public GameObject LocalPlayer;
    public Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float TimerAmount = 5f;
    private bool RunRespawnTimer = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //GameCanvas.SetActive(true);
    }

    private void Update()
    {
        CheckInput();
        PingText.text = "Ping: ";

        if(RunRespawnTimer)
        {
            StartRespawn();
        }
    }

    private void RespawnLocation()
    {
        //float RandomValue = Random.Range(-5, 5f);
        //LocalPlayer.transform.localPosition = new Vector2(RandomValue, 3f);
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
            //LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.All);
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

    public void PlayerDied()
    {
        if (!IsServer) return;

        LoadGameOverSceneClientRpc();
    }

    [ClientRpc]
    private void LoadGameOverSceneClientRpc()
    {
        // This will be called on all clients
        SceneManager.LoadScene(gameOverSceneName);
    }
}