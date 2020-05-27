using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class GameOverSceneLauncher : MonoBehaviour
{
    #region Private Serializable Fields
    [Tooltip("The UI panel to display user scores")]
    [SerializeField]
    private GameObject PlayerListPanel;

    [Tooltip("The prefab of an user entry")]
    [SerializeField]
    private GameObject PlayerScorePrefab;

    [Tooltip("The UI Text to display user's highest score")]
    [SerializeField]
    private Text highestScore;
    #endregion

    private Player[] currentPlayers;
    private bool playerScoreSet=false;
    private double spacing;

    void Awake()
    {
        //Play game over background music
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayGameOverSound();

        //Caculate the distance between each player entry using their screen size
        spacing = Screen.height * 0.125;

        //If not single player mode, then don't display highest score and enable PhotonNetwork
        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            highestScore.enabled = false;
            PhotonNetwork.AutomaticallySyncScene = true;
            currentPlayers = PhotonNetwork.PlayerList;
        }
        //If player played game as a guest, don't display highest score
        if(GoogleSignInDemo._instance.isGuest)
        {
            highestScore.enabled = false;
        }
            
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Multiplayer Mode
        if(!StartSceneLauncher._instance.singlePlayerMode)
        {
            if (CheckEverybodyUploadScore() && !playerScoreSet)
            {
                setPlayersScores();
            }
        }
        //Singleplayer Mode + Not A Guest
        else if(!GoogleSignInDemo._instance.isGuest)
        {
            GameObject entry = Instantiate(PlayerScorePrefab, PlayerListPanel.transform);
            entry.GetComponent<PlayerScore>().Initialize(DatabaseScript.displayName, GameManager._instance.score);
            highestScore.text = "Highest Score" + Environment.NewLine + DatabaseScript.playerScore.ToString();
        }
        //Singleplayer Mode + Guest
        else
        {
            GameObject entry = Instantiate(PlayerScorePrefab, PlayerListPanel.transform);
            entry.GetComponent<PlayerScore>().Initialize("Guest Player", GameManager._instance.score);
        }
        
        if (CrossPlatformInputManager.GetButtonDown("RestartButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().StopGameOverSound();
            SceneManager.LoadScene("StartScene");
        }
    }


    private void setPlayersScores()
    {
        double spacingSum = 0;
        foreach(Player p in currentPlayers)
        {
            GameObject entry = Instantiate(PlayerScorePrefab, PlayerListPanel.transform);
            entry.transform.localPosition = Vector3.zero;
            entry.transform.localRotation = Quaternion.identity;
            entry.transform.localScale = Vector3.one;
            entry.transform.localPosition = new Vector3(entry.transform.localPosition.x, entry.transform.localPosition.y - (float)spacingSum, entry.transform.localPosition.z);
            entry.GetComponent<PlayerScore>().Initialize(p.NickName, (int)p.CustomProperties["Score"]);
            spacingSum += spacing;
        }
        playerScoreSet = true;
    }

    private bool CheckEverybodyUploadScore()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (string.IsNullOrEmpty(p.CustomProperties["Score"].ToString()))
            {
                return false;
            }
        }
        return true;
    }
}
