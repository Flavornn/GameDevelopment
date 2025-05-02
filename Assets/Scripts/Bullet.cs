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
        rb = GetComponent<Rigidbody2D>();

        if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
        {
            Vector2 direction = (Vector2)photonView.InstantiationData[0];
            rb.velocity = direction * force; // velocity from data
            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot);
        }

        transform.localScale = new Vector3(bulletStats._bulletSize, bulletStats._bulletSize, 1f);
        StartCoroutine("DestroyByTime");
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
                target.RPC("ReduceHealth", RpcTarget.All, (int)bulletStats._bulletDamage);
            }

            if (target.tag == "Ground")
            {
                this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
            }

            this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        }
    }
}