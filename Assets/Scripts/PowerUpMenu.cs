using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class PowerUpMenu : MonoBehaviour
{
    [Header("References")]
    public PowerUps powerUpsSystem; // ScriptableObject reference
    public Stats playerStats; // Player's stats asset
    public GameObject powerUpPrefab;
    public Transform contentParent;
    public int powerUpsToShow = 3;

    private List<PowerUps.PowerUpType> availablePowerUps;

    void Start()
    {
        availablePowerUps = System.Enum.GetValues(typeof(PowerUps.PowerUpType))
            .Cast<PowerUps.PowerUpType>()
            .Where(p => !powerUpsSystem.activePowerUps.Contains(p))
            .ToList();

        CreatePowerUpSelection();
    }

    void CreatePowerUpSelection()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        var selectedPowerUps = new List<PowerUps.PowerUpType>();
        for (int i = 0; i < Mathf.Min(powerUpsToShow, availablePowerUps.Count); i++)
        {
            int randomIndex = Random.Range(0, availablePowerUps.Count);
            selectedPowerUps.Add(availablePowerUps[randomIndex]);
            availablePowerUps.RemoveAt(randomIndex);
        }

        foreach (var powerUpType in selectedPowerUps)
        {
            CreatePowerUpButton(powerUpType);
        }
    }

    void CreatePowerUpButton(PowerUps.PowerUpType powerUpType)
    {
        GameObject buttonObj = Instantiate(powerUpPrefab, contentParent);
        TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        Button confirmButton = buttonObj.GetComponentInChildren<Button>();

        texts[0].text = SplitCamelCase(powerUpType.ToString());
        texts[1].text = powerUpsSystem.GetPowerUpDescription(powerUpType);

        confirmButton.onClick.AddListener(() =>
        {
            powerUpsSystem.TogglePowerUp(powerUpType, playerStats); // Pass playerStats
        });
    }

    public void ResetPowerUps()
    {
        powerUpsSystem.ResetAllPowerUps(playerStats);
        availablePowerUps = System.Enum.GetValues(typeof(PowerUps.PowerUpType))
            .Cast<PowerUps.PowerUpType>()
            .ToList();
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

    public void ConfirmSelections()
    {
        gameObject.SetActive(false);
    }
}