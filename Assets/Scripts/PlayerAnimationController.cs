using System.Collections;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator), typeof(Player))]
public class PlayerAnimationController : MonoBehaviourPunCallbacks
{
    private Animator animator;
    private Player player;
    private bool isHurt = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        // Only the client that “owns” this photonView should push parameter changes.
        if (!photonView.IsMine || isHurt)
            return;

        float speed = Mathf.Abs(player.rb.velocity.x);
        bool grounded = player.isGrounded;
        bool m1pressed = Input.GetMouseButton(0);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("m1pressed", m1pressed);
    }

    // Called when hit by a bullet—only on the owner side.
    public void TriggerHurt()
    {
        if (!photonView.IsMine || isHurt) return;

        isHurt = true;
        animator.SetTrigger("BulletHit");
        StartCoroutine(RecoverFromHurt(0.5f));
    }

    private IEnumerator RecoverFromHurt(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHurt = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;
        if (other.CompareTag("Bullet"))
            TriggerHurt();
    }
}
