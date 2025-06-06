using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerUIUpdater : MonoBehaviour
{
    [Header("Player 1 UI")]
    public TMP_Text player1NameText;
    public TMP_Text player1HealthText;
    public TMP_Text player1AmmoText;
    public GameObject[] player1Dots;

    [Header("Player 2 UI")]
    public TMP_Text player2NameText;
    public TMP_Text player2HealthText;
    public TMP_Text player2AmmoText;
    public GameObject[] player2Dots;

    [Header("Round Timer")]
    public TMP_Text roundTimerText;

    private PlayerHealth[] allPlayers;

    void Start()
    {
        UpdateRoundDots();
    }


    void Update()
    {
        // Find all instantiated PlayerHealth components in the scene
        allPlayers = FindObjectsOfType<PlayerHealth>();

        // Loop through both and check .photonView.IsMine
        foreach (PlayerHealth p in allPlayers)
        {
            if (p == null || p.photonView == null) continue;

            // Get this player’s actor number and NickName
            int actor = p.photonView.OwnerActorNr;
            string nick = p.photonView.Owner.NickName;

            // Fetch current health and max health from PlayerHealth / Stats
            float health = p.GetCurrentHealth();
            Shooting shooting = p.GetComponent<Shooting>();
            int currentAmmo = (shooting != null) ? GetCurrentAmmo(shooting) : 0;
            int maxAmmo = (shooting != null) ? shooting.shootingStats._maxAmmo : 0;

            // Assign to the correct UI panel based on actor number (1 or 2)
            if (actor == 1)
            {
                // Display NickName, Health, and Ammo for Player 1
                if (player1NameText != null)
                    player1NameText.text = nick;

                if (player1HealthText != null)
                    player1HealthText.text = $"{health}";

                if (player1AmmoText != null)
                    player1AmmoText.text = $"{currentAmmo}";
            }
            else if (actor == 2)
            {
                // Display NickName, Health, and Ammo for Player 2
                if (player2NameText != null)
                    player2NameText.text = nick;

                if (player2HealthText != null)
                    player2HealthText.text = $"{health}";

                if (player2AmmoText != null)
                    player2AmmoText.text = $"{currentAmmo}";
            }
        }

        if (roundTimerText != null && GameManager.Instance != null)
        {
            float t = Mathf.Max(0f, GameManager.Instance.GetRemainingTime());
            int seconds = Mathf.FloorToInt(t);
            roundTimerText.text = $"{seconds}";
        }
    }
    private int GetCurrentAmmo(Shooting shooting)
    {
        var field = typeof(Shooting).GetField(
            "currentAmmo",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        return (field != null) ? (int)field.GetValue(shooting) : 0;
    }

    void UpdateRoundDots()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int wins = player.CustomProperties.ContainsKey("Wins") ? (int)player.CustomProperties["Wins"] : 0;
            bool isPlayer1 = player.ActorNumber == 1;

            GameObject[] dots = isPlayer1 ? player1Dots : player2Dots;

            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i] != null)
                    dots[i].SetActive(i < wins);
            }
        }
    }

}
