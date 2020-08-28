﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObj;

    [HideInInspector]
    public float currentHatTime;
    Rigidbody rb;

    [Header("Components")]
    public Player photonPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // checks for inputs on controls every frame, if Move detects an input it will change velocity
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;
        // use a kinematic control for horizontal movement but leave vertical movement unaffected for jumping
        rb.velocity = new Vector3(x, rb.velocity.y, z);
    }

    void TryJump()
    {
        // This method of jumping seems to implement a raycast to detect distance from ground to avoid a double jump
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 0.7f))
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;

        //give first player hat

        // disable physics for other players except yourself - let the network send position data 
        if(!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
        if (id == 1)
            GameManager.instance.GiveHat(id, true);
    }

    public void SetHat(bool hasHat)
    {
        hatObj.SetActive(hasHat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                if(GameManager.instance.CanGetHat())
                {
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }
}
