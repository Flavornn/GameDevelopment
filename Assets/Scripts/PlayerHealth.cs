using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
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


        GameManager.Instance.EnableRespawn();

    }

    private void Respawn()
    {
        rb.gravityScale = 1;
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

            plMove.DisableInput = false;
    }

    private void UpdateHealthUI()
    {
        FillImage.fillAmount = currentHealth / playerStats._health;
    }
}