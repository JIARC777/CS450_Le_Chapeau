using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickUpTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;

    // Thinking this is meant as a global access point but not a singleton
    public static GameManager instance;

    private void Awake()
    {
        // every time a new GM is created it overwrites the old one
        instance = this;
    }

    void Start()
    {
       // PhotonView PV = GetComponent<PhotonView>();
       // Debug.Log(PV);
        // Get a list of all players and announce you are in game
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        // Guessing this is just a failsafe against errors in joining
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayer()
    {
        // Choose a random spawn point and instantiate the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        //get player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    public PlayerController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        //remove hat from current player
        if (!initialGive)
        {
            GetPlayer(playerWithHat).SetHat(false);
        }

        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickUpTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > hatPickUpTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame (int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
