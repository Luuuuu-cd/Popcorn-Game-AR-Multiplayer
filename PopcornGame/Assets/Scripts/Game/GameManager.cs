using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


//This script controls the game process
public class GameManager : MonoBehaviour
{
    #region Private Serializable Fields
    [SerializeField]
    private GameObject cloudAnchorInfoPanel;
    [SerializeField]
    private Camera firstPersonCamera;
    [SerializeField]
    private GameObject popcornMachinePrefab;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text finalScoreText;
    [SerializeField]
    private Text gameInfoText;
    [SerializeField]
    private SpawnManager spawnManager;
    [SerializeField]
    private GameObject countDownPanel;
    [SerializeField]
    private GameObject bonusRoundInformPanelGameObject;
    #endregion


    private float timeLeft = 90;
    private int readyPlayers = 0;
    private bool isCountingDown = false;
    private bool isBonusRound = false;
    private GameObject popcornMachineGameObject;
    private Hashtable scoreHash;

    public int score = 0;
    public bool startSpawn = false;
    public bool singlePlayerReady = false;
    public Dictionary<string, int> scoreDictionary = new Dictionary<string, int>();
    public static GameManager _instance { get; set; }

    #region MonoBehaviour CallBacks
    private void Awake()
    {
        //Make game manger a singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().StopBGM();
        bonusRoundInformPanelGameObject.SetActive(false);
        GetComponent<BonusRound>().enabled = false;

        //Add different food with different score to the dictionary
        scoreDictionary.Add("RegularPopcorn", 1);
        scoreDictionary.Add("BananaPeel", -5);
        scoreDictionary.Add("Donut", 3);
        scoreDictionary.Add("ChocolatePopcorn", 1);
        scoreDictionary.Add("MatchaPopcorn", 1);
        scoreDictionary.Add("StrawberryPopcorn", 1);
        scoreDictionary.Add("HoneyPopcorn", 1);

        spawnManager.enabled = false;

        //If not single player mode, create hashtable for scores of all the players
        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            scoreHash = new Hashtable();
            PhotonNetwork.LocalPlayer.SetCustomProperties(scoreHash);
        }
     
        GameStart();
        countDownPanel.SetActive(false);
    }

    void Update()
    {
        //If the game starts and didn't reach the time limit, keep processing touches
        if (startSpawn && timeLeft>=0)
        {
            ProcessTouches();
            timeLeft -= Time.deltaTime;
        }
        //enable bonus round if there is only 30 seconds left
        if(timeLeft<30)
        {
            if (!isBonusRound)
            {
                bonusRoundInformPanelGameObject.SetActive(true);
                isBonusRound = true;   
            }
            Invoke("DisableBonusRoundPanel", 2);
            StartBonusRound();
        }
        if (timeLeft < 0)
        {
            startSpawn = false;
            // store highest score into the database 
            updateDatabase(); 
            GameOver();
        }
        timerText.text = "Time: " + (int)timeLeft + " s";
        scoreText.text = "Score: " + score;

        //If all the players are ready, start the game

        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            if (readyPlayers == PhotonNetwork.CurrentRoom.PlayerCount && !isCountingDown)
            {
                cloudAnchorInfoPanel.SetActive(false);
                isCountingDown = true;
                countDownPanel.SetActive(true);
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayCountDownSound();
                Invoke("StartSpawnPopcorns", 4);
            }
        }
        else if(singlePlayerReady&&!isCountingDown)
        {
            isCountingDown = true;
            countDownPanel.SetActive(true);
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayCountDownSound();
            Invoke("StartSpawnPopcorns", 4);
        }
    }
    #endregion

    #region Private Methods
    void GameStart()
    {
        CreatePopcornMachine();
    }

    void StartSpawnPopcorns()
    {
        startSpawn = true;
        countDownPanel.SetActive(false);
        spawnManager.enabled = true;
    }

    void StartBonusRound()
    {
        spawnManager.BonusRound = true;
        GetComponent<BonusRound>().enabled = true;
    }

    void GameOver()
    {
        //Destroy all the popcorns and popcorn machine, go to game over scene
        foreach (GameObject existGameObject in FindObjectsOfType<GameObject>())
        {
            if (existGameObject.tag == ("Popcorn")|| existGameObject.tag == ("Donut") || existGameObject.tag == ("Fan") || existGameObject.tag == ("Ink") )
            {
                Destroy(existGameObject);
            }
        }
        Destroy(popcornMachineGameObject);

        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            scoreHash.Add("Score", score);
            PhotonNetwork.LocalPlayer.SetCustomProperties(scoreHash);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOverScene");
            }
        }
        else
        {
            SceneManager.LoadScene("GameOverScene");
        }

    }

    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount < 1 ||
            (touch = Input.GetTouch(Input.touchCount - 1)).phase != TouchPhase.Began)
        {
            return;
        }
       
        RaycastHit rayHit;
        Ray ray = firstPersonCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
       
        if (Physics.Raycast(ray, out rayHit))
        {
            //user touches a collider
            if (rayHit.collider != null)
            {
                rayHit.collider.enabled = false;
            }
            //Check if user touches a popcorn
            if (rayHit.collider.gameObject.CompareTag("RegularPopcorn")|| rayHit.collider.gameObject.CompareTag("ChocolatePopcorn")|| 
                rayHit.collider.gameObject.CompareTag("HoneyPopcorn")|| rayHit.collider.gameObject.CompareTag("MatchaPopcorn")|| 
                rayHit.collider.gameObject.CompareTag("StrawberryPopcorn")|| rayHit.collider.gameObject.CompareTag("Donut")||
                rayHit.collider.gameObject.CompareTag("BananaPeel"))
            {
                int score;
                if (scoreDictionary.TryGetValue(rayHit.collider.gameObject.tag, out score))
                {
                    CollectPopcorn(score, rayHit.collider.gameObject);
                }

            }
            else if (rayHit.collider.gameObject.tag == "Fan")
            {
                GameItemManager._instance.itemInventory = "Fan";
                CollectPopcorn(0, rayHit.collider.gameObject);
            }
            else if (rayHit.collider.gameObject.tag == "Ink")
            {
                GameItemManager._instance.itemInventory = "Ink";
                CollectPopcorn(0, rayHit.collider.gameObject);
            }
        }
    }

    void CreatePopcornMachine()
    {
        Vector3 popcornMachinePos = new Vector3(0, 5, 0);
        popcornMachineGameObject=Instantiate(popcornMachinePrefab, popcornMachinePos, Quaternion.identity);
        popcornMachineGameObject.tag = "popcornMachine";
    }


    void updateDatabase()
    {
        DatabaseScript database = new DatabaseScript();
        if (DatabaseScript.userID != null && DatabaseScript.userID != "")
        {
            int prevHighest = DatabaseScript.playerScore;
            if (score > prevHighest)
            {
                database.updateUserScoreToDatabase(score);
            }
        }
    }

    void DisableBonusRoundPanel()
    {
        bonusRoundInformPanelGameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    public void CollectPopcorn(int scoreToAdd, GameObject itemToCollect)
    {
        Behaviour halo = (Behaviour)itemToCollect.GetComponent("Halo");
        //Check whether this popcorn is close enough to catch or not, if yes, score increases and the popcorn is destroyed
        if (halo.enabled == true)
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayCollectPopcornSound();
            if (scoreToAdd < 0)
            {
                gameInfoText.text = itemToCollect.tag + " Collected! Score " + scoreToAdd.ToString();
            }
            else if (scoreToAdd == 0)
            {
                gameInfoText.text = itemToCollect.tag + " Collected!";
            }
            else
            {
                gameInfoText.text = itemToCollect.tag + " Collected! Score + " + scoreToAdd.ToString();
            }

            score += scoreToAdd;
            Destroy(itemToCollect);

            if (!StartSceneLauncher._instance.singlePlayerMode)
            {
                //Send the message to other players to destroy the popcorn in their local copy of the game
                PhotonView _photonView = itemToCollect.GetComponent<PhotonView>();
                _photonView.RPC("DestroyPopcorn", RpcTarget.Others, _photonView.ViewID);
            }

        }
    }
    #endregion

    [PunRPC]
    void IncreaseReadyPlayers()
    {
        readyPlayers++;
    }
}