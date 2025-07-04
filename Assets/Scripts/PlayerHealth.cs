using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public Stats playerStats; // Reference to Stats ScriptableObject
    private float currentHealth; // Local health value for each player instance
    public PowerUps powerUpsSystem; // Add reference to PowerUps ScriptableObject

    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public Player plMove;

    private void Awake()
    {
        playerStats = Instantiate(playerStats);

        if (playerStats != null)
        {
            currentHealth = playerStats._health;
            Debug.Log($"Player {photonView.ViewID} initialized with health: {currentHealth}");

            // Load and apply power-ups
            if (PhotonNetwork.CurrentRoom != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PlayerPowerUps") &&
                powerUpsSystem != null)
            {
                var powerUpsData = (object[])PhotonNetwork.CurrentRoom.CustomProperties["PlayerPowerUps"];
                foreach (object[] powerUpInfo in powerUpsData)
                {
                    int powerUpType = (int)powerUpInfo[0];
                    int playerActorNumber = (int)powerUpInfo[1];

                    if (playerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        powerUpsSystem.TogglePowerUp((PowerUps.PowerUpType)powerUpType, playerStats);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("PlayerStats reference is missing in PlayerHealth!");
        }
    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        Debug.Log($"ReduceHealth called on player {photonView.ViewID}, current health: {currentHealth}, damage: {amount}");
        ModifyHealth(amount);
        
        if (currentHealth <= 0)
        {
            Debug.Log($"Player {photonView.ViewID} health reached 0.");
            //photonView.RPC("Dead", RpcTarget.All);
            
            if (photonView.IsMine)
            {
                GameManager.Instance.HandlePlayerDeath(photonView.Owner.ActorNumber);
            }
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    private void CheckHealth()
    {
        if (photonView.IsMine && currentHealth <= 0)
        {
            GameManager.Instance.HandlePlayerDeath(photonView.Owner.ActorNumber);
        }
    }

    public void EnableInput()
    {
        plMove.DisableInput = false;
    }

    private void ModifyHealth(int amount)
    {
        float previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats._health);
        Debug.Log($"Player {photonView.ViewID} health modified from {previousHealth} to {currentHealth}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("OutOfBounds"))
        {
            GameManager.Instance.HandlePlayerDeath(photonView.Owner.ActorNumber);
        }
    }
}