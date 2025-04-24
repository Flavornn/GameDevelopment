using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "PowerUps", menuName = "ScriptableObjects/PowerUps")]
public class PowerUps : ScriptableObject
{
    public enum PowerUpType
    {
        RapidFire,
        PrecisionBurst,
        HeavyRounds,
        ArmorPiercing,
        ExtendedMagazine,
        SpeedLoader,
        NitroBoost,
        ReinforcedPlating,
        AgileShooter,
        HighVelocity,
        HollowPoints,
        MicroRounds,
        VampiricRounds,
        AdrenalineRush,
        SniperConfiguration,
        ShotgunSpread,
        TankMode,
        GlassCannon,
        BulletHell,
        GuerrillaTactics
    }

    [Header("Active Power-Ups")]
    public List<PowerUpType> activePowerUps = new List<PowerUpType>();

    private Dictionary<PowerUpType, Action<Stats>> powerUpEffects = new Dictionary<PowerUpType, Action<Stats>>
    {
        // Fire Rate Modifiers
        { PowerUpType.RapidFire, (stats) => {
            stats._fireRate *= 1.5f;
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 0.8f);
        }},
        { PowerUpType.PrecisionBurst, (stats) => {
            stats._fireRate *= 2f;
            stats._reloadTime *= 1.5f;
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 0.7f);
        }},

        // Damage Modifiers
        { PowerUpType.HeavyRounds, (stats) => {
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 1.4f);
            stats._bulletSpeed *= 0.75f;
        }},
        { PowerUpType.ArmorPiercing, (stats) => {
            stats._bulletDamage += 5;
            stats._fireRate *= 0.8f;
        }},

        // Ammo/Reload Modifiers
        { PowerUpType.ExtendedMagazine, (stats) => {
            stats._maxAmmo = Mathf.RoundToInt(stats._maxAmmo * 1.5f);
            stats._reloadTime *= 1.25f;
        }},
        { PowerUpType.SpeedLoader, (stats) => {
            stats._reloadTime *= 0.6f;
            stats._maxAmmo = Mathf.RoundToInt(stats._maxAmmo * 0.8f);
        }},

        // Movement Modifiers
        { PowerUpType.NitroBoost, (stats) => {
            stats._speed *= 1.3f;
            stats._health = Mathf.RoundToInt(stats._health * 0.85f);
        }},
        { PowerUpType.ReinforcedPlating, (stats) => {
            stats._health += 50;
            stats._speed *= 0.8f;
        }},
        { PowerUpType.AgileShooter, (stats) => {
            stats._speed *= 1.2f;
            stats._fireRate *= 1.2f;
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 0.9f);
        }},

        // Bullet Physics Modifiers
        { PowerUpType.HighVelocity, (stats) => {
            stats._bulletSpeed *= 1.4f;
            stats._bulletSize *= 0.8f;
        }},
        { PowerUpType.HollowPoints, (stats) => {
            stats._bulletDamage += 3;
            stats._maxAmmo -= 2;
        }},
        { PowerUpType.MicroRounds, (stats) => {
            stats._bulletSize *= 0.7f;
            stats._maxAmmo += 3;
            stats._bulletDamage -= 2;
        }},

        // Special Effects
        { PowerUpType.VampiricRounds, (stats) => {
            stats._health += 20;
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 0.85f);
        }},
        { PowerUpType.AdrenalineRush, (stats) => {
            stats._speed *= 1.25f;
            stats._jumpHeight *= 1.3f;
            stats._health = Mathf.RoundToInt(stats._health * 0.9f);
        }},
        { PowerUpType.SniperConfiguration, (stats) => {
            stats._bulletSize *= 1.5f;
            stats._bulletSpeed *= 1.3f;
            stats._fireRate *= 0.6f;
        }},
        { PowerUpType.ShotgunSpread, (stats) => {
            stats._bulletDamage += 4;
            stats._maxAmmo = Mathf.RoundToInt(stats._maxAmmo * 0.6f);
        }},
        { PowerUpType.TankMode, (stats) => {
            stats._health += 75;
            stats._speed *= 0.7f;
            stats._reloadTime *= 1.4f;
        }},
        { PowerUpType.GlassCannon, (stats) => {
            stats._bulletDamage *= 2;
            stats._health = Mathf.RoundToInt(stats._health * 0.4f);
        }},
        { PowerUpType.BulletHell, (stats) => {
            stats._fireRate *= 2f;
            stats._maxAmmo += 5;
            stats._bulletDamage = Mathf.RoundToInt(stats._bulletDamage * 0.6f);
        }},
        { PowerUpType.GuerrillaTactics, (stats) => {
            stats._reloadTime *= 0.5f;
            stats._maxAmmo = Mathf.RoundToInt(stats._maxAmmo * 0.7f);
            stats._speed *= 1.15f;
        }}
    };

    public void TogglePowerUp(PowerUpType type, Stats targetStats)
    {
        if (activePowerUps.Contains(type))
        {
            RevertPowerUp(type, targetStats);
            activePowerUps.Remove(type);
        }
        else
        {
            ApplyPowerUp(type, targetStats);
            activePowerUps.Add(type);
        }
    }

    public string GetPowerUpDescription(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.RapidFire: return "Fire Rate +50%\nBullet Damage -20%";
            case PowerUpType.PrecisionBurst: return "Fire Rate +100%\nReload Time +50%\nBullet Damage -30%";
            case PowerUpType.HeavyRounds: return "Bullet Damage +40%\nBullet Speed -25%";
            case PowerUpType.ArmorPiercing: return "Bullet Damage +5\nFire Rate -20%";
            case PowerUpType.ExtendedMagazine: return "Max Ammo +50%\nReload Time +25%";
            case PowerUpType.SpeedLoader: return "Reload Time -40%\nMax Ammo -20%";
            case PowerUpType.NitroBoost: return "Speed +30%\nHealth -15%";
            case PowerUpType.ReinforcedPlating: return "Health +50\nSpeed -20%";
            case PowerUpType.AgileShooter: return "Speed +20%\nFire Rate +20%\nBullet Damage -10%";
            case PowerUpType.HighVelocity: return "Bullet Speed +40%\nBullet Size -20%";
            case PowerUpType.HollowPoints: return "Bullet Damage +3\nMax Ammo -2";
            case PowerUpType.MicroRounds: return "Bullet Size -30%\nMax Ammo +3\nBullet Damage -2";
            case PowerUpType.VampiricRounds: return "Health +20\nBullet Damage -15%";
            case PowerUpType.AdrenalineRush: return "Speed +25%\nJump Height +30%\nHealth -10%";
            case PowerUpType.SniperConfiguration: return "Bullet Size +50%\nBullet Speed +30%\nFire Rate -40%";
            case PowerUpType.ShotgunSpread: return "Bullet Damage +4\nMax Ammo -40%";
            case PowerUpType.TankMode: return "Health +75\nSpeed -30%\nReload Time +40%";
            case PowerUpType.GlassCannon: return "Bullet Damage +100%\nHealth -60%";
            case PowerUpType.BulletHell: return "Fire Rate +100%\nMax Ammo +5\nBullet Damage -40%";
            case PowerUpType.GuerrillaTactics: return "Reload Time -50%\nMax Ammo -30%\nSpeed +15%";
            default: return "Unknown Power-Up";
        }
    }

    private void ApplyPowerUp(PowerUpType type, Stats targetStats)
    {
        if (powerUpEffects.TryGetValue(type, out Action<Stats> effect))
        {
            effect.Invoke(targetStats);
            Debug.Log($"Applied: {type}");
        }
    }

    private void RevertPowerUp(PowerUpType type, Stats targetStats)
    {
        switch (type)
        {
            case PowerUpType.RapidFire:
                targetStats._fireRate /= 1.5f;
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 0.8f);
                break;

            case PowerUpType.PrecisionBurst:
                targetStats._fireRate /= 2f;
                targetStats._reloadTime /= 1.5f;
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 0.7f);
                break;

            case PowerUpType.HeavyRounds:
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 1.4f);
                targetStats._bulletSpeed /= 0.75f;
                break;

            case PowerUpType.ArmorPiercing:
                targetStats._bulletDamage -= 5;
                targetStats._fireRate /= 0.8f;
                break;

            case PowerUpType.ExtendedMagazine:
                targetStats._maxAmmo = Mathf.RoundToInt(targetStats._maxAmmo / 1.5f);
                targetStats._reloadTime /= 1.25f;
                break;

            case PowerUpType.SpeedLoader:
                targetStats._reloadTime /= 0.6f;
                targetStats._maxAmmo = Mathf.RoundToInt(targetStats._maxAmmo / 0.8f);
                break;

            case PowerUpType.NitroBoost:
                targetStats._speed /= 1.3f;
                targetStats._health = Mathf.RoundToInt(targetStats._health / 0.85f);
                break;

            case PowerUpType.ReinforcedPlating:
                targetStats._health -= 50;
                targetStats._speed /= 0.8f;
                break;

            case PowerUpType.AgileShooter:
                targetStats._speed /= 1.2f;
                targetStats._fireRate /= 1.2f;
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 0.9f);
                break;

            case PowerUpType.HighVelocity:
                targetStats._bulletSpeed /= 1.4f;
                targetStats._bulletSize /= 0.8f;
                break;

            case PowerUpType.HollowPoints:
                targetStats._bulletDamage -= 3;
                targetStats._maxAmmo += 2;
                break;

            case PowerUpType.MicroRounds:
                targetStats._bulletSize /= 0.7f;
                targetStats._maxAmmo -= 3;
                targetStats._bulletDamage += 2;
                break;

            case PowerUpType.VampiricRounds:
                targetStats._health -= 20;
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 0.85f);
                break;

            case PowerUpType.AdrenalineRush:
                targetStats._speed /= 1.25f;
                targetStats._jumpHeight /= 1.3f;
                targetStats._health = Mathf.RoundToInt(targetStats._health / 0.9f);
                break;

            case PowerUpType.SniperConfiguration:
                targetStats._bulletSize /= 1.5f;
                targetStats._bulletSpeed /= 1.3f;
                targetStats._fireRate /= 0.6f;
                break;

            case PowerUpType.ShotgunSpread:
                targetStats._bulletDamage -= 4;
                targetStats._maxAmmo = Mathf.RoundToInt(targetStats._maxAmmo / 0.6f);
                break;

            case PowerUpType.TankMode:
                targetStats._health -= 75;
                targetStats._speed /= 0.7f;
                targetStats._reloadTime /= 1.4f;
                break;

            case PowerUpType.GlassCannon:
                targetStats._bulletDamage /= 2;
                targetStats._health = Mathf.RoundToInt(targetStats._health / 0.4f);
                break;

            case PowerUpType.BulletHell:
                targetStats._fireRate /= 2f;
                targetStats._maxAmmo -= 5;
                targetStats._bulletDamage = Mathf.RoundToInt(targetStats._bulletDamage / 0.6f);
                break;

            case PowerUpType.GuerrillaTactics:
                targetStats._reloadTime /= 0.5f;
                targetStats._maxAmmo = Mathf.RoundToInt(targetStats._maxAmmo / 0.7f);
                targetStats._speed /= 1.15f;
                break;

            default:
                Debug.LogWarning($"No reversal defined for: {type}");
                break;
        }
    }

    public void ResetAllPowerUps(Stats targetStats)
    {
        foreach (var powerUp in activePowerUps)
        {
            RevertPowerUp(powerUp, targetStats);
        }
        activePowerUps.Clear();
    }
}