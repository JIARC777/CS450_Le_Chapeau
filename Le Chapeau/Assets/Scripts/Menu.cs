using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

   // [Header("Components")]
   // public PhotonView photonViewer;

    // Start is called before the first frame update
    void Start()
    {
        // disable buttons when we arent connected to the server
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    // toggle which menu you want based on input selection
    void SetScreen (GameObject screen)
    {
        //deactivate
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        // only enable requested screen
        screen.SetActive(true);
    }

    // set room name based on field test and send to network manager
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    // request room name based on field test and send to network manager
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    // Get name from name field and send to Photon Network
    public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text; 
    }

    // This function handles the updating of the player list in the lobby screen and permissions for starting the game
    [PunRPC]
    public void UpdateLobbyUI ()
    {
       // playerListText.text = "Test";
        // display players currently in lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }
        // only host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else startGameButton.interactable = false;
    }

    // Swap Menu Screens when room is joined
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        // since this means a player joined the lobby, update the lobby screen (note this gets called from every computer as it joins, so it updates as people join)
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        // This gets called for everyone in the room by default so no need to RPC
        UpdateLobbyUI();
    }

    // When you leave the lobby, disconnect and go back to main screen
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    // Since this button is only used by master client, pressing it allows them to change scenes for everyone
    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
}

