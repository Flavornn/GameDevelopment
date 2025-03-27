using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Stats shootingStats;

    [Header("Local Instance Variables")]
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canFire = true;
    private float timer;
    private float timeBetweenShots; // Local instance of cooldown time

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        CalculateTimeBetweenShots(); // Initialize local instance value
    }

    private void CalculateTimeBetweenShots()
    {
        // Convert fire rate (shots per second) to time between shots (seconds)
        timeBetweenShots = 1f / shootingStats._fireRate;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HandleAiming();
        HandleFiring();
    }

    private void HandleAiming()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - firePoint.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private void HandleFiring()
    {
        // Handle cooldown
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenShots)
            {
                canFire = true;
                timer = 0f;
            }
        }

        // Handle firing input
        if (Input.GetButtonDown("Fire1") && canFire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        canFire = false;
        PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
    }

    // Call this if stats change during gameplay
    public void RefreshShootingStats()
    {
        CalculateTimeBetweenShots();
    }
}