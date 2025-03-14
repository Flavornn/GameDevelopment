using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public bool MoveDir = false; // false = right, true = left;
    public float MoveSpeed;
    public float DestroyTime;
    public float BulletDamage;

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    private void Awake()
    {
        //StartCoroutine("DestroyByTime");
    }

    IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(DestroyTime);
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
        rb.velocity = new Vector2 (direction.x, direction.y).normalized * force;
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
            transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!photonView.IsMine)
            return;

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if( target != null && (!target.IsMine || target.IsRoomView))
        {
            if(target.tag == "Player")
            {
                target.RPC("ReduceHealth", RpcTarget.All, BulletDamage);
            }

            this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        }
    }
}
