using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// change inherited class to MonoBehaviorPunCallbacks to allow for functions that demonstrate commununication with master server
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance; 
    // use a simple singleton to allow for global access
    void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Connect to photon servers on start
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();        
    }
    // Overrriding call when connecting to master server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection To Master Server Established");
    }

    //create a room from a given name
    public void CreateRoom (string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    // search for a room based on given name
    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Override message when a room is created
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }

    //use photon to change the scene - added features useful for multiplayer
    [PunRPC] //Allow this function to be called as an RPC
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
