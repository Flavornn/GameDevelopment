using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Shooting : NetworkBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Stats shootingStats;

    [Header("Local Instance Variables")]
    private Camera mainCam;
    private Vector3 mousePos;
    private float timeBetweenShots;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;
    private float lastShotTime;

    private void Start()
    {
        mainCam = Camera.main;
        CalculateTimeBetweenShots();
        currentAmmo = shootingStats._maxAmmo;
        lastShotTime = -timeBetweenShots;
    }

    private void CalculateTimeBetweenShots()
    {
        timeBetweenShots = 1f / shootingStats._fireRate;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleAiming();
        HandleFiring();
        HandleReloading();
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
        if (isReloading) return;

        if (Input.GetButton("Fire1") && currentAmmo > 0)
        {
            if (Time.time - lastShotTime >= timeBetweenShots)
            {
                Fire();
                lastShotTime = Time.time;
            }
        }
    }

    private void Fire()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // Request server to spawn bullet
        FireServerRpc(direction, firePoint.position, firePoint.rotation);
    }

    [ServerRpc]
    private void FireServerRpc(Vector2 direction, Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().Initialize(direction, shootingStats);

        currentAmmo--;

        if (currentAmmo <= 0)
        {
            isReloading = true;
            reloadTimer = 0f;
        }
    }

    private void HandleReloading()
    {
        if (!isReloading) return;

        reloadTimer += Time.deltaTime;
        if (reloadTimer >= shootingStats._reloadTime)
        {
            currentAmmo = shootingStats._maxAmmo;
            isReloading = false;
        }
    }

    public void RefreshShootingStats()
    {
        CalculateTimeBetweenShots();
        currentAmmo = Mathf.Min(currentAmmo, shootingStats._maxAmmo);
    }
}