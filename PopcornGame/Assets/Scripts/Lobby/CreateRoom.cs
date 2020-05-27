using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;


public class CreateRoom : MonoBehaviourPunCallbacks
{
    
    private List<RoomInfo> roomList;
    private bool roomNameAlreadyExists = false;

    #region Private Serializable Fields

    [Tooltip("The UI InputField to let the user enter maximum number of players of the room")]
    [SerializeField]
    private InputField MaxPlayer;

    [Tooltip("The UI InputField to let the user enter the room name")]
    [SerializeField]
    private InputField RoomName;

    [Tooltip("The UI InputField to let the user enter Nickname")]
    [SerializeField]
    private InputField PlayerNameInput;

    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    [Tooltip("The UI Label to inform the user the input they entered is not valid")]
    [SerializeField]
    private GameObject notificationText;

    [Tooltip("To set the maximum number of players in a room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 6;
    #endregion

    #region Private Fields
    string gameVersion = "1";
    #endregion


    #region MonoBehaviour CallBacks
    void Start()
    {
        //Use automaticallySyncScene to make sure all the players load the same level
        PhotonNetwork.AutomaticallySyncScene = true;

        roomList = new List<RoomInfo>();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        notificationText.SetActive(false);
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("CreateRoomButton")) 
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            Connect();
        }
        if (CrossPlatformInputManager.GetButtonDown("BackButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("MultiPlayerStartScene");
        }

        //Keep detecting whether room name user input already exists
        foreach (RoomInfo entry in roomList)
        {
            if (RoomName.text == entry.Name)
            {
                progressLabel.SetActive(false);
                controlPanel.SetActive(true);
                notificationText.GetComponent<Text>().text = "Room Name Already Exists";
                notificationText.SetActive(true);
                roomNameAlreadyExists = true;
                break;
            }
            else
            {
                roomNameAlreadyExists = false;
            }
        }
    }

    #endregion


    #region Public Methods
    public void Connect()
    {

        //Check user input is valid or not
        if (CheckInput())
        {
            PlayerNameInput.GetComponent<PlayerNameInputField>().SetPlayerName(PlayerNameInput.text);
            PhotonNetwork.LocalPlayer.NickName = PlayerNameInput.text;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            notificationText.SetActive(false);

            //If already connected, join the loby
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinLobby();
            }
            //If not connected, connect to the photon server first
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        
    }

    public bool CheckInput()
    {
        int number;
        //Check player name is not empty
        if (string.IsNullOrEmpty(PlayerNameInput.text))
        {
            notificationText.GetComponent<Text>().text = "Player name is invalid";
            notificationText.SetActive(true);
            return false;
        }
        //Check maxPlayer is a integer
        else if (!int.TryParse(MaxPlayer.text, out number))
        {
            notificationText.GetComponent<Text>().text = "Maximum player is invalid";
            notificationText.SetActive(true);
            return false;
        }
        //Check maxPlayer is > 1
        else if (number <= 1)
        {
            notificationText.GetComponent<Text>().text = "Maximum player can't be " + number;
            notificationText.SetActive(true);
            return false;
        }
        //Check maxPlayer is < 6
        else if (number > maxPlayersPerRoom)
        {
            notificationText.GetComponent<Text>().text = "Maximum player can't exceed " + maxPlayersPerRoom;
            notificationText.SetActive(true);
            return false;
        }
        //Check room name is not empty
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

    public void Create()
    {
        //Create a room with all the information
        byte maxPlayers;
        string roomNameText = RoomName.text;
        string maxPlayerText = MaxPlayer.text;
        byte.TryParse(maxPlayerText, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 1, maxPlayersPerRoom);
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomNameText, options, null);
    }
    #endregion


    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnRoomListUpdate(List<RoomInfo> list)
    {
        roomList = list;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        if (!roomNameAlreadyExists)
        {
            Create();
        }
    }

    public override void OnJoinedRoom()
    {
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }
    #endregion
}