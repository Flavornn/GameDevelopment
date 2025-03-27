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
        // Initialize local health with the max health from Stats
        currentHealth = playerStats._health;
        UpdateHealthUI();
    }

    [PunRPC]
    public void ReduceHealth(float amount)
    {
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
        if (photonView.IsMine && currentHealth <= 0)
        {
            GameManager.Instance.EnableRespawn();
            plMove.DisableInput = true;
            this.GetComponent<PhotonView>().RPC("Dead", RpcTarget.All);
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
    }

    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 1;
        bc.enabled = true;
        sr.enabled = true;
        PlayerCanvas.SetActive(true);
        currentHealth = playerStats._health; // Reset to max health from Stats
        UpdateHealthUI();
    }

    private void ModifyHealth(float amount)
    {
        if (photonView.IsMine)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, playerStats._health); // Ensure health doesn't go below 0 or above max
            UpdateHealthUI();
            CheckHealth();
        }
    }

    private void UpdateHealthUI()
    {
        FillImage.fillAmount = currentHealth / playerStats._health;
    }
}