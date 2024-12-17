using UnityEngine;
public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;

    public GameObject player;
    public float bulletSpeed = 50;
    Vector2 lookDirection;
    float lookAngle;

    void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(player.transform.position.x, player.transform.position.y);
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, lookAngle);

        GameObject bulletClone = Instantiate(bullet);
        bulletClone.transform.position = firePoint.position;
        bulletClone.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        Rigidbody2D rb = bulletClone.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;

    }
}