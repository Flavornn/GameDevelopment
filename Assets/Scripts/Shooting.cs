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
    private float timeBetweenShots;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;
    private float lastShotTime;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
        if (!photonView.IsMine) return;

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

        // Automatic fire while holding mouse button
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
        object[] bulletData = new object[] { direction };
        PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation, 0, bulletData);
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

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int CurrentAmmo => currentAmmo;

    public void RefreshShootingStats()
    {
        CalculateTimeBetweenShots();
        currentAmmo = Mathf.Min(currentAmmo, shootingStats._maxAmmo);
    }
}