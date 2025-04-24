using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveInput;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Stats playerStats;
    public Text PlayerNameText;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public bool isGrounded = false;
    public GameObject BulletObject;
    public Transform FirePoint;
    public bool DisableInput = false;

    private void Update()
    {
        if (!DisableInput)
        {
            CheckInput();
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(BulletObject, FirePoint.position, Quaternion.identity);
        if (sr.flipX)
        {
            //bullet.GetComponent<Bullet>().ChangeDir_Left();
        }
    }

    private void CheckInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * playerStats._speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, playerStats._jumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            FlipTrue();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            FlipFalse();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Shoot();
        }
    }

    private void FlipTrue()
    {
        sr.flipX = true;
    }

    private void FlipFalse()
    {
        sr.flipX = false;
    }
}