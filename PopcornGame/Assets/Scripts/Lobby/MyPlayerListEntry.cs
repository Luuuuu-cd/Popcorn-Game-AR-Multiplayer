using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


public class MyPlayerListEntry : MonoBehaviour
{
    [SerializeField]
    private Text PlayerNameText;

    [SerializeField]
    private GameObject PlayerReadyButton;

    [SerializeField]
    private Image PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;


    #region UNITY
    void Awake()
    {
        isPlayerReady = false;
        PlayerReadyImage.enabled = false;
    }

    void Update()
    {
        //If player ready button clicked, change local state and update the ready information to photon network
        if (CrossPlatformInputManager.GetButtonDown(PlayerNameText.text+"PlayerReadyButton"))
        {
            isPlayerReady = !isPlayerReady;
            SetLocalPlayerReady(isPlayerReady);
            Hashtable props = new Hashtable() {{ PopcornGame.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            if (PhotonNetwork.IsMasterClient)
            {
                FindObjectOfType<RoomManager>().LocalPlayerPropertiesUpdated();
            }
        }

    }

    //Initialize the entry with ID, name and ready button
    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerReadyButton.GetComponent<ButtonHandler>().Name = PlayerNameText.text+"PlayerReadyButton";
    }
    
    //Change ready button text and display ready image
    public void SetLocalPlayerReady(bool playerReady)
    {
        isPlayerReady = playerReady;
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
        PlayerReadyImage.enabled = playerReady;
    }

    //This is called by join Room Manager script to display ready image of other player entries
    public void SetOtherPlayerReady(bool playerReady)
    {
        isPlayerReady = playerReady;
        PlayerReadyImage.enabled = playerReady;
    }
    #endregion
}
