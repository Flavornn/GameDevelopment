using UnityEngine;
public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    private Camera mainCam;
    private Vector3 mousePos;
    public bool canFire;
    private float timer;
    public float timeBetweenShots;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 rotation = mousePos - firePoint.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if(!canFire)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenShots)
            {
                canFire = true;
                timer = 0;

            }
        }

        if (Input.GetButtonDown("Fire1") && canFire)
        {
            canFire = false;
            Instantiate(bullet, firePoint.position, Quaternion.identity);
        }

    }
}