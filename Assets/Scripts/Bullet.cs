using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Stats bulletStats;
    private Rigidbody2D rb;
    private Vector2 direction;

    public void Initialize(Vector2 shootDirection, Stats stats)
    {
        bulletStats = stats;
        rb = GetComponent<Rigidbody2D>();
        direction = shootDirection;

        // Apply initial force
        rb.velocity = direction * bulletStats._bulletSpeed;

        // Set correct rotation
        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot);

        // Scale bullet
        transform.localScale = Vector3.one * bulletStats._bulletSize;

        StartCoroutine(DestroyByTime());
    }

    IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().ReduceHealth((int)bulletStats._bulletDamage);
        }
        Destroy(gameObject);
    }
}