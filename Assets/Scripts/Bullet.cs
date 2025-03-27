using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public bool MoveDir = false; // false = right, true = left;
    public Stats bulletStats; // Add reference to Stats+

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    private void Awake()
    {
        // Scale bullet based on size stat
        transform.localScale = new Vector3(bulletStats._bulletSize, bulletStats._bulletSize, 1f);
        StartCoroutine("DestroyByTime");
    }

    IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(2f); // You might want to add this to Stats too
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeDir_Left()
    {
        MoveDir = true;
    }

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot + 180);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (!MoveDir)
        {
            transform.Translate(Vector2.right * bulletStats._bulletSpeed * Time.deltaTime); // Use stats for speed
        }
        else
        {
            transform.Translate(Vector2.left * bulletStats._bulletSpeed * Time.deltaTime); // Use stats for speed
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if (target != null && (!target.IsMine || target.IsRoomView))
        {
            if (target.tag == "Player")
            {
                target.RPC("ReduceHealth", RpcTarget.All, bulletStats._bulletDamage); // Use stats for damage
            }

            if (target.tag == "Ground")
            {
                this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
            }

            this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        }
    }
}