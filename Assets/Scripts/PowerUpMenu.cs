using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Photon.Pun;

public class PowerUpMenu : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public PowerUps powerUpsSystem;
    public Stats playerStats;
    public GameObject powerUpPrefab;
    public Transform contentParent;
    public int powerUpsToShow = 3;

    private List<PowerUps.PowerUpType> availablePowerUps;
    private bool isDeadPlayer = false;
    private List<GameObject> currentButtons = new List<GameObject>();

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

        // Create unique instance of stats if this is the dead player
        if (isDeadPlayer)
        {
            playerStats = Instantiate(playerStats);
        }

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

        // Only select powerups if this is the dead player (master client could do this instead)
        if (isDeadPlayer)
        {
            for (int i = 0; i < Mathf.Min(powerUpsToShow, availablePowerUps.Count); i++)
            {
                int randomIndex = Random.Range(0, availablePowerUps.Count);
                selectedPowerUps.Add(availablePowerUps[randomIndex]);
                availablePowerUps.RemoveAt(randomIndex);
            }
        }
        else
        {
            // Show placeholder buttons for the alive player
            for (int i = 0; i < powerUpsToShow; i++)
            {
                selectedPowerUps.Add(PowerUps.PowerUpType.RapidFire); // Default type, will be hidden
            }
        }

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
                powerUpsSystem.TogglePowerUp(powerUpType, playerStats);
                photonView.RPC("RPC_ConfirmSelections", RpcTarget.All);
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
    public void RPC_ConfirmSelections()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
        gameObject.SetActive(false);
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