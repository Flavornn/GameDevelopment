using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public Stats playerStats; // Reference to Stats ScriptableObject
    private float currentHealth; // Local health value for each player instance

    public Image FillImage;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    public Player plMove;
    public GameObject PlayerCanvas;

    private void Awake()
    {
        if (playerStats != null)
        {
            currentHealth = playerStats._health;
            UpdateHealthUI();
        }
        else
        {
            Debug.LogError("PlayerStats reference missing!");
        }
    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            ModifyHealth(amount);
        }
    }

    private void CheckHealth()
    {
        if (photonView.IsMine && currentHealth <= 0)
        {
            GameManager.Instance.HandlePlayerDeath(photonView.Owner.ActorNumber);
            // Rest of your existing code
        }
    }

    public void EnableInput()
    {
        plMove.DisableInput = false;
    }

    [PunRPC]
    private void Dead()
    {
        rb.gravityScale = 0;
        bc.enabled = false;
        sr.enabled = false;
        PlayerCanvas.SetActive(false);

        if (photonView.IsMine)
        {
            GameManager.Instance.EnableRespawn();
        }
    }

    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 5;
        bc.enabled = true;
        sr.enabled = true;
        PlayerCanvas.SetActive(true);
        currentHealth = playerStats._health; // Reset to max health from Stats
        UpdateHealthUI();
    }

    private void ModifyHealth(int amount) // Changed from float to int
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats._health);
        UpdateHealthUI();
        CheckHealth();

        if (photonView.IsMine)
        {
            plMove.DisableInput = false;
        }
    }

    private void UpdateHealthUI()
    {
        FillImage.fillAmount = currentHealth / playerStats._health;
    }
}