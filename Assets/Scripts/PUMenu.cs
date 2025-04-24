using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class PowerUpMenu : MonoBehaviour
{
    [Header("References")]
    public PowerUps powerUpsSystem;
    public GameObject powerUpPrefab;
    public Transform contentParent;
    public int powerUpsToShow = 3;
    public float verticalSpacing = 10f;
    public Stats playerStats;

    private List<PowerUps.PowerUpType> availablePowerUps;

    void Start()
    {
        // Initialize list of all power-up types except those already active
        availablePowerUps = System.Enum.GetValues(typeof(PowerUps.PowerUpType))
            .Cast<PowerUps.PowerUpType>()
            .Where(p => !powerUpsSystem.activePowerUps.Contains(p))
            .ToList();

        CreatePowerUpSelection();
    }

    void CreatePowerUpSelection()
    {
        // Select random unique power-ups
        var selectedPowerUps = new List<PowerUps.PowerUpType>();
        for (int i = 0; i < Mathf.Min(powerUpsToShow, availablePowerUps.Count); i++)
        {
            int randomIndex = Random.Range(0, availablePowerUps.Count);
            selectedPowerUps.Add(availablePowerUps[randomIndex]);
            availablePowerUps.RemoveAt(randomIndex);
        }

        // Create UI elements with index tracking
        for (int i = 0; i < selectedPowerUps.Count; i++)
        {
            CreatePowerUpButton(selectedPowerUps[i], i);
        }
    }

    void CreatePowerUpButton(PowerUps.PowerUpType powerUpType, int index)
    {
        GameObject buttonObj = Instantiate(powerUpPrefab, contentParent);
        TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        Button confirmButton = buttonObj.GetComponentInChildren<Button>();

        // Set UI content
        texts[0].text = FormatPowerUpName(powerUpType);
        texts[1].text = powerUpsSystem.GetPowerUpDescription(powerUpType);

        // Set button action
        confirmButton.onClick.AddListener(() =>
        {
            powerUpsSystem.TogglePowerUp(powerUpType, playerStats);
        });

    }

    string FormatPowerUpName(PowerUps.PowerUpType type)
    {
        string name = type.ToString();
        // Insert spaces before capital letters (except first character)
        return System.Text.RegularExpressions.Regex.Replace(name,
            "([a-z])([A-Z])",
            "$1 $2");
    }
}