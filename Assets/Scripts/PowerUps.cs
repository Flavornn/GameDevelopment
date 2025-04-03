using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [System.Serializable]
    public struct PowerUpEntry
    {
        public string powerUpName;
        public bool isActive;
        [HideInInspector] public bool wasActive; // For state tracking
    }

    [Header("Core Configuration")]
    public Stats playerStats; // Assign this ONCE in Inspector

    [Header("Power-Up List")]
    public List<PowerUpEntry> powerUps = new List<PowerUpEntry>();

    private void OnValidate()
    {
        if (playerStats == null) return;
        ApplyPowerUps();
    }

    private void ApplyPowerUps()
    {
        for (int i = 0; i < powerUps.Count; i++)
        {
            var entry = powerUps[i];

            if (entry.isActive && !entry.wasActive)
            {
                ApplyPowerUp(entry.powerUpName);
                entry.wasActive = true;
            }
            else if (!entry.isActive && entry.wasActive)
            {
                RevertPowerUp(entry.powerUpName);
                entry.wasActive = false;
            }

            powerUps[i] = entry;
        }
    }

    private void ApplyPowerUp(string powerUpName)
    {
        switch (powerUpName)
        {
           
            case "Rapid Fire":
                playerStats._fireRate *= 1.5f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 0.8f);
                break;

            case "Precision Burst":
                playerStats._fireRate *= 2f;
                playerStats._reloadTime *= 1.5f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 0.7f);
                break;

            
            case "Heavy Rounds":
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 1.4f);
                playerStats._bulletSpeed *= 0.75f;
                break;

            case "Armor Piercing":
                playerStats._bulletDamage += 5;
                playerStats._fireRate *= 0.8f;
                break;

            
            case "Extended Magazine":
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo * 1.5f);
                playerStats._reloadTime *= 1.25f;
                break;

            case "Speed Loader":
                playerStats._reloadTime *= 0.6f;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo * 0.8f);
                break;

            
            case "Nitro Boost":
                playerStats._speed *= 1.3f;
                playerStats._health = Mathf.RoundToInt(playerStats._health * 0.85f);
                break;

            case "Reinforced Plating":
                playerStats._health += 50;
                playerStats._speed *= 0.8f;
                break;

            case "Agile Shooter":
                playerStats._speed *= 1.2f;
                playerStats._fireRate *= 1.2f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 0.9f);
                break;

            
            case "High Velocity":
                playerStats._bulletSpeed *= 1.4f;
                playerStats._bulletSize *= 0.8f;
                break;

            case "Hollow Points":
                playerStats._bulletDamage += 3;
                playerStats._maxAmmo -= 2;
                break;

            case "Micro Rounds":
                playerStats._bulletSize *= 0.7f;
                playerStats._maxAmmo += 3;
                playerStats._bulletDamage -= 2;
                break;

            
            case "Vampiric Rounds":
                playerStats._health += 20;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 0.85f);
                break;

            case "Adrenaline Rush":
                playerStats._speed *= 1.25f;
                playerStats._jumpHeight *= 1.3f;
                playerStats._health = Mathf.RoundToInt(playerStats._health * 0.9f);
                break;

            
            case "Sniper Configuration":
                playerStats._bulletSize *= 1.5f;
                playerStats._bulletSpeed *= 1.3f;
                playerStats._fireRate *= 0.6f;
                break;

            case "Shotgun Spread":
                playerStats._bulletDamage += 4;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo * 0.6f);
                break;

            case "Tank Mode":
                playerStats._health += 75;
                playerStats._speed *= 0.7f;
                playerStats._reloadTime *= 1.4f;
                break;

            case "Glass Cannon":
                playerStats._bulletDamage *= 2;
                playerStats._health = Mathf.RoundToInt(playerStats._health * 0.4f);
                break;

            case "Bullet Hell":
                playerStats._fireRate *= 2f;
                playerStats._maxAmmo += 5;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage * 0.6f);
                break;

            case "Guerrilla Tactics":
                playerStats._reloadTime *= 0.5f;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo * 0.7f);
                playerStats._speed *= 1.15f;
                break;
        }
    }

    private void RevertPowerUp(string powerUpName)
    {
        switch (powerUpName)
        {
            case "Rapid Fire":
                playerStats._fireRate /= 1.5f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 0.8f);
                break;

            case "Precision Burst":
                playerStats._fireRate /= 2f;
                playerStats._reloadTime /= 1.5f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 0.7f);
                break;

            case "Heavy Rounds":
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 1.4f);
                playerStats._bulletSpeed /= 0.75f;
                break;

            case "Armor Piercing":
                playerStats._bulletDamage -= 5;
                playerStats._fireRate /= 0.8f;
                break;

            case "Extended Magazine":
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo / 1.5f);
                playerStats._reloadTime /= 1.25f;
                break;

            case "Speed Loader":
                playerStats._reloadTime /= 0.6f;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo / 0.8f);
                break;

            case "Nitro Boost":
                playerStats._speed /= 1.3f;
                playerStats._health = Mathf.RoundToInt(playerStats._health / 0.85f);
                break;

            case "Reinforced Plating":
                playerStats._health -= 50;
                playerStats._speed /= 0.8f;
                break;

            case "Agile Shooter":
                playerStats._speed /= 1.2f;
                playerStats._fireRate /= 1.2f;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 0.9f);
                break;

            case "High Velocity":
                playerStats._bulletSpeed /= 1.4f;
                playerStats._bulletSize /= 0.8f;
                break;

            case "Hollow Points":
                playerStats._bulletDamage -= 3;
                playerStats._maxAmmo += 2;
                break;

            case "Micro Rounds":
                playerStats._bulletSize /= 0.7f;
                playerStats._maxAmmo -= 3;
                playerStats._bulletDamage += 2;
                break;

            case "Vampiric Rounds":
                playerStats._health -= 20;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 0.85f);
                break;

            case "Adrenaline Rush":
                playerStats._speed /= 1.25f;
                playerStats._jumpHeight /= 1.3f;
                playerStats._health = Mathf.RoundToInt(playerStats._health / 0.9f);
                break;

            case "Sniper Configuration":
                playerStats._bulletSize /= 1.5f;
                playerStats._bulletSpeed /= 1.3f;
                playerStats._fireRate /= 0.6f;
                break;

            case "Shotgun Spread":
                playerStats._bulletDamage -= 4;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo / 0.6f);
                break;

            case "Tank Mode":
                playerStats._health -= 75;
                playerStats._speed /= 0.7f;
                playerStats._reloadTime /= 1.4f;
                break;

            case "Glass Cannon":
                playerStats._bulletDamage /= 2;
                playerStats._health = Mathf.RoundToInt(playerStats._health / 0.4f);
                break;

            case "Bullet Hell":
                playerStats._fireRate /= 2f;
                playerStats._maxAmmo -= 5;
                playerStats._bulletDamage = Mathf.RoundToInt(playerStats._bulletDamage / 0.6f);
                break;

            case "Guerrilla Tactics":
                playerStats._reloadTime /= 0.5f;
                playerStats._maxAmmo = Mathf.RoundToInt(playerStats._maxAmmo / 0.7f);
                playerStats._speed /= 1.15f;
                break;
        }
    }
}