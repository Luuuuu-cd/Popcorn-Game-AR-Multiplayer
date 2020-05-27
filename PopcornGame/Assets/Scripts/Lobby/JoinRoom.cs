using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class JoinRoom : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    [Tooltip("The InputField to input player's Nickname")]
    [SerializeField]
    private InputField PlayerNameInput;

    [Tooltip("The InputField to input room name")]
    [SerializeField]
    private InputField RoomName;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    [Tooltip("The UI Text to inform the user's input is not valid")]
    [SerializeField]
    private GameObject notificationText;

    #endregion


    #region Private Fields
    string gameVersion = "1";
    #endregion


    #region MonoBehaviour CallBacks
    void Start()
    {
        //Use automaticallySyncScene to make sure all the players load the same level
        PhotonNetwork.AutomaticallySyncScene = true;

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        notificationText.SetActive(false);
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("JoinRoomButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            Connect();
        }
        if (CrossPlatformInputManager.GetButtonDown("BackButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("MultiPlayerStartScene");
        }
    }

    #endregion


    #region Public Methods



    public void Connect()
    {
        
        if(CheckInput())
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            notificationText.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Join();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        
    }

    public void Join()
    {
        string roomNameText = RoomName.text;
        PhotonNetwork.JoinRoom(roomNameText);
    }

    public bool CheckInput()
    {
        if (string.IsNullOrEmpty(PlayerNameInput.text))
        {
            notificationText.GetComponent<Text>().text = "Player name is invalid";
            notificationText.SetActive(true);
            return false;
        }
        else if (string.IsNullOrEmpty(RoomName.text))
        {
            notificationText.GetComponent<Text>().text = "Room name is invalid";
            notificationText.SetActive(true);
            Debug.Log("Room name is invalid");
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion


    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Join();  
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        notificationText.GetComponent<Text>().text = "Room not exist";
        notificationText.SetActive(true);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion

}