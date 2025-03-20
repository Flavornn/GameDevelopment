using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    public float moveSpeed;
    public float jumpForce;
    public float moveInput;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public PhotonView PhotonView;
    public Text PlayerNameText;
    public GameObject PlayerCamera;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public bool isGrounded = false;

    public GameObject BulletObject;
    public Transform FirePoint;

    public bool DisableInput = false;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            //PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            PlayerNameText.text = photonView.Owner.NickName;
            PlayerNameText.color = Color.cyan;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(photonView.IsMine && !DisableInput)
        {
            CheckInput();
        }

    }

    private void Shoot()
    {
        if(sr.flipX == false)
        {
            GameObject obj = PhotonNetwork.Instantiate(BulletObject.name, new Vector2(FirePoint.position.x, FirePoint.position.y), Quaternion.identity, 0);
        }

        if(sr.flipX == true)
        {
            GameObject obj = PhotonNetwork.Instantiate(BulletObject.name, new Vector2(FirePoint.position.x, FirePoint.position.y), Quaternion.identity, 0);
            obj.GetComponent<PhotonView>().RPC("ChangeDir_Left", RpcTarget.All);
        }
    }

    private void CheckInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        var move = new Vector3(Input.GetAxisRaw("Horizontal"),0);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            photonView.RPC("FlipTrue", RpcTarget.All);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("FlipFalse", RpcTarget.All);
        }

        //if(Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    Shoot();
        //}
    }

    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
    }
}
