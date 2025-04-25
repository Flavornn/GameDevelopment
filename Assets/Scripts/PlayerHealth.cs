using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public Stats playerStats; 
    private float currentHealth; 

    public Image FillImage;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    public Player plMove;
    public GameObject PlayerCanvas;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        currentHealth = playerStats._health;
        UpdateHealthUI();
    }

    public void ReduceHealth(int amount)
    {
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
            GameManager.Instance.EnableRespawn();
            plMove.DisableInput = true;
    }

    public void EnableInput()
    {
        plMove.DisableInput = false;
    }

    private void Dead()
    {
        rb.gravityScale = 0;
        bc.enabled = false;
        sr.enabled = false;
        PlayerCanvas.SetActive(false);


        //GameManager.Instance.EnableRespawn();

    }

    private void ModifyHealth(int amount)
    {
        if (!IsServer) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats._health);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Dead();
            GameManager.Instance.PlayerDied();
        }
    }

    private void UpdateHealthUI()
    {
        FillImage.fillAmount = currentHealth / playerStats._health;
    }
}