using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}
public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    // create a global instance
    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {
        //loop through all containers
        for (int i = 0; i < playerContainers.Length; i++)
        {
            PlayerUIContainer container = playerContainers[i];
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else
                container.obj.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
       UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        for (int i = 0; i < GameManager.instance.players.Length; ++i)
        {
            if (GameManager.instance.players[i] != null)
                playerContainers[i].hatTimeSlider.value = GameManager.instance.players[i].currentHatTime;
        }
    }

    public void SetWinText (string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
}
