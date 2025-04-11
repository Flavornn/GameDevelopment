using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{

    public float moveInput;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Stats playerStats; // Add reference to Stats

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
        if (photonView.IsMine)
        {
            PlayerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            PlayerNameText.text = photonView.Owner.NickName;
            PlayerNameText.color = Color.cyan;
        }
    }

    void Update()
    {
        if (photonView.IsMine && !DisableInput)
        {
            CheckInput();
        }
    }

    private void Shoot()
    {
        if (sr.flipX == false)
        {
            GameObject obj = PhotonNetwork.Instantiate(BulletObject.name, new Vector2(FirePoint.position.x, FirePoint.position.y), Quaternion.identity, 0);
        }

        if (sr.flipX == true)
        {
            GameObject obj = PhotonNetwork.Instantiate(BulletObject.name, new Vector2(FirePoint.position.x, FirePoint.position.y), Quaternion.identity, 0);
            obj.GetComponent<PhotonView>().RPC("ChangeDir_Left", RpcTarget.All);
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
            photonView.RPC("FlipTrue", RpcTarget.All);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("FlipFalse", RpcTarget.All);
        }
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