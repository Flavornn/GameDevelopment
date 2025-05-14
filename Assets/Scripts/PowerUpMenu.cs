using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Photon.Pun;
using System.Collections;

public class PowerUpMenu : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public PowerUps powerUpsSystem;
    public Stats playerStats;
    public GameObject powerUpPrefab;
    public Transform contentParent;
    public int powerUpsToShow = 3;

    private List<PowerUps.PowerUpType> availablePowerUps;
    private PhotonView photonView;
    private bool isDeadPlayer = false;
    private List<GameObject> currentButtons = new List<GameObject>();

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView)
        {
            photonView = gameObject.AddComponent<PhotonView>();
        }
    }

    void Start()
    {
        InitializePowerUpMenu();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        InitializePowerUpMenu();
    }

    void InitializePowerUpMenu()
    {
        // Clear existing buttons
        foreach (var button in currentButtons)
        {
            if (button != null) Destroy(button);
        }
        currentButtons.Clear();

        // Check if this is the dead player
        isDeadPlayer = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("DeadPlayer") &&
                      (int)PhotonNetwork.CurrentRoom.CustomProperties["DeadPlayer"] == PhotonNetwork.LocalPlayer.ActorNumber;

        InitializeAvailablePowerUps();
        CreatePowerUpSelection();
    }

    void InitializeAvailablePowerUps()
    {
        availablePowerUps = System.Enum.GetValues(typeof(PowerUps.PowerUpType))
            .Cast<PowerUps.PowerUpType>()
            .Where(p => !powerUpsSystem.activePowerUps.Contains(p))
            .ToList();
    }

    void CreatePowerUpSelection()
    {
        // Create buttons for all players to see
        var selectedPowerUps = new List<PowerUps.PowerUpType>();

        // Only select powerups if this is the dead player
        if (isDeadPlayer)
        {
            for (int i = 0; i < Mathf.Min(powerUpsToShow, availablePowerUps.Count); i++)
            {
                int randomIndex = Random.Range(0, availablePowerUps.Count);
                selectedPowerUps.Add(availablePowerUps[randomIndex]);
                availablePowerUps.RemoveAt(randomIndex);
            }

            // Sync the selected power-ups across the network
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_SyncSelectedPowerUps", RpcTarget.All, selectedPowerUps.Select(p => (int)p).ToArray());
            }
        }

        foreach (var powerUpType in selectedPowerUps)
        {
            CreatePowerUpButton(powerUpType);
        }
    }

    [PunRPC]
    private void RPC_SyncSelectedPowerUps(int[] powerUpTypes)
    {
        var selectedPowerUps = powerUpTypes.Select(t => (PowerUps.PowerUpType)t).ToList();
        foreach (var powerUpType in selectedPowerUps)
        {
            CreatePowerUpButton(powerUpType);
        }
    }

    void CreatePowerUpButton(PowerUps.PowerUpType powerUpType)
    {
        GameObject buttonObj = Instantiate(powerUpPrefab, contentParent);
        currentButtons.Add(buttonObj);

        TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        Button confirmButton = buttonObj.GetComponentInChildren<Button>();

        if (isDeadPlayer)
        {
            // Real powerup for dead player
            texts[0].text = SplitCamelCase(powerUpType.ToString());
            texts[1].text = powerUpsSystem.GetPowerUpDescription(powerUpType);
            confirmButton.interactable = true;

            confirmButton.onClick.AddListener(() =>
            {
                // Only apply power-up if we're the dead player
                if (isDeadPlayer)
                {
                    photonView.RPC("RPC_ApplyPowerUp", RpcTarget.All, (int)powerUpType, PhotonNetwork.LocalPlayer.ActorNumber);
                }
            });
        }
        else
        {
            // Placeholder for alive player
            texts[0].text = "Power Selection";
            texts[1].text = "Waiting for opponent to choose...";
            confirmButton.interactable = false;
        }
    }

    [PunRPC]
    private void RPC_ApplyPowerUp(int powerUpType, int playerActorNumber)
    {
        // Only apply the power-up if this is the player who died
        if (playerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PowerUps.PowerUpType type = (PowerUps.PowerUpType)powerUpType;
            powerUpsSystem.TogglePowerUp(type, playerStats);
            
            // Store which player has this power-up
            var powerUpData = new object[] { powerUpType, playerActorNumber };
            var currentPowerUps = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PlayerPowerUps") 
                ? (object[])PhotonNetwork.CurrentRoom.CustomProperties["PlayerPowerUps"] 
                : new object[0];
                
            var newPowerUps = currentPowerUps.Concat(new[] { powerUpData }).ToArray();
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerPowerUps", newPowerUps } });
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadGameAfterDelay());
        }
    }

    private IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel("Game");
    }

    public void ResetPowerUps()
    {
        if (isDeadPlayer)
        {
            powerUpsSystem.ResetAllPowerUps(playerStats);
            InitializeAvailablePowerUps();
        }
    }

    string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            input,
            "([A-Z])",
            " $1",
            System.Text.RegularExpressions.RegexOptions.Compiled
        ).Trim();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("DeadPlayer"))
        {
            bool wasDeadPlayer = isDeadPlayer;
            isDeadPlayer = (int)propertiesThatChanged["DeadPlayer"] == PhotonNetwork.LocalPlayer.ActorNumber;

            // Refresh UI if status changed
            if (wasDeadPlayer != isDeadPlayer)
            {
                InitializePowerUpMenu();
            }
        }
    }
}