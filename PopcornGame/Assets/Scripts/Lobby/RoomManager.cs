using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields
    [Tooltip("Room name")]
    [SerializeField]
    private Text roomNameText;

    [Tooltip("The UI Panel to place player entries")]
    [SerializeField]
    private GameObject PlayerListPanel;

    [Tooltip("The prefab of one player entry")]
    [SerializeField]
    private GameObject PlayerListEntryPrefab;

    [Tooltip("The button to start the game")]
    [SerializeField]
    private GameObject StartGameButton;

    #endregion
    private double spacingSum;
    private double spacing;
    private Dictionary<int, GameObject> playerListEntries;
    private int countOfPlayers;
    private PhotonView _photonView;

    #region MonoBehaviour CallBacks
    public void Awake()
    {
        //Distance between entries is calculated by the screen size
        spacingSum = 0;
        spacing = Screen.height * 0.1;

        //Use automaticallySyncScene to make sure all the players load the same level
        PhotonNetwork.AutomaticallySyncScene = true;

        //Display the name of the room created by the host
        roomNameText.text = "Room Name\n" + PhotonNetwork.CurrentRoom.Name;

        //Host is the first player in the room
        countOfPlayers = 1;
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        //Refresh to display the current player list
        RefreshPlayerList();
    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("LeaveRoomButton"))
        {
            PhotonNetwork.LeaveRoom();
        }
        if (CrossPlatformInputManager.GetButtonDown("StartGameButton"))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    public override void OnLeftRoom()
    {
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        playerListEntries.Clear();
        playerListEntries = null;
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        SceneManager.LoadScene("MultiPlayerStartScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            countOfPlayers++;
            _photonView.RPC("SetPlayerNum", RpcTarget.Others, countOfPlayers);
        }
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            countOfPlayers--;
            _photonView.RPC("SetPlayerNum",RpcTarget.Others, countOfPlayers);
        }
        RefreshPlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(PopcornGame.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<MyPlayerListEntry>().SetOtherPlayerReady((bool)isPlayerReady);
            }
        }
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #region Public Methods
    public void RefreshPlayerList()
    {
        spacingSum = 0;

        //Clear the panel
        if(playerListEntries!=null)
        {
            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }
        }
        playerListEntries = new Dictionary<int, GameObject>();
        

        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            count++;

            //Create one entry for each player in the PhotonNetwork PlayerList
            GameObject entry = Instantiate(PlayerListEntryPrefab);

            //If the entry creating is for the local player, enable PlayerReadyButtons
            if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {

                entry.transform.Find("PlayerListEntryBackgroundPanel").transform.Find("PlayerReadyButton").gameObject.SetActive(false);
            }

            //Place the entry according to the spacingSum
            entry.transform.position = new Vector3(entry.transform.position.x, entry.transform.position.y - (float)spacingSum, entry.transform.position.z);
            entry.transform.SetParent(PlayerListPanel.transform);
            entry.transform.localScale = Vector3.one;

            //Initialize the entry using ActorNumber and NickName
            entry.GetComponent<MyPlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            //If this player is ready, enable the ready image
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(PopcornGame.PLAYER_READY, out isPlayerReady))
            {
                Debug.Log("Room Manager: " + p.NickName + " " + isPlayerReady);
                entry.GetComponent<MyPlayerListEntry>().SetOtherPlayerReady((bool)isPlayerReady);
            }

            //Add the new entry to the playerListEntries
            playerListEntries.Add(p.ActorNumber, entry);
            spacingSum += spacing;
        }

        //If every player is ready, enable start game button
        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        //If the player just entered the room, upload custom property PLAYER_READY
        if(!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(PopcornGame.PLAYER_READY))
        {
            Hashtable props = new Hashtable
            {
                {PopcornGame.PLAYER_READY, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }  
    }

    //This is called when the ready button in local player is clicked
    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    private bool CheckPlayersReady()
    {
        //Only display the start game button if the client is the Master Client
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(PopcornGame.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    [PunRPC]
    void SetPlayerNum(int playerNum)
    {
        countOfPlayers = playerNum;
    }
}
