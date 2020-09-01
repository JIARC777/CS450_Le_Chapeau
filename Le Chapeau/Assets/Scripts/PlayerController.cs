using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObj;

    [HideInInspector]
    public float currentHatTime;
   

    [Header("Components")]
    public Rigidbody rb;
    public Player photonPlayer;

    // Start is called before the first frame update
    void Start()
    {
       // rb = this.GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //if you are host check if a player has won
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
        if (photonView.IsMine)
        {
            // checks for inputs on controls every frame, if Move detects an input it will change velocity
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();

            //track amount of time hat worn
            if (hatObj.activeInHierarchy)
                currentHatTime+= Time.deltaTime;
        }
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

   void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Collision Detected");
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

    // Rather than set up observable through photon view, access stream directly
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(currentHatTime);
        else if (stream.IsReading)
            currentHatTime = (float)stream.ReceiveNext();
    }
}
