using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System;


//This script controls the game process
public class GameManager : MonoBehaviour
{
    public Text DebugText;
    public Text CloudAnchorText;
    public Camera firstPersonCamera;
    public GameObject popcornMachinePrefab;
    public Animator GameOverAnimator;
    private GameObject popcornMachineGameObject;
    public int score = 0;
    private float timeLeft = 60;
    public Text scoreText;
    public Text timerText;
    public Text finalScoreText;
    public SpawnManager spawnManager;
    public GameObject countDownPanel;
    private int readyPlayers=0;
    public static bool startSpawn = false;
    private bool isCountingDown = false;
    private bool isBonusRound = false;
    public bool singlePlayerReady = false;
    private Hashtable scoreHash;
    public GameObject bonusRoundInformPanelGameObject;

    private Dictionary<string, string> mainSceneLatency;
    private DateTime pingTime;
    private double latency;

    public Text gameInfoText;

    public Dictionary<string, int> scoreDictionary = new Dictionary<string, int>();

    void Start()
    {
        
        bonusRoundInformPanelGameObject.SetActive(false);
        GetComponent<BonusRound>().enabled = false;
        scoreDictionary.Add("RegularPopcorn", 1);
        //scoreDictionary.Add("Cookie", 3);
        scoreDictionary.Add("BananaPeel", -5);
        scoreDictionary.Add("Donut", 3);
        scoreDictionary.Add("ChocolatePopcorn", 1);
        scoreDictionary.Add("MatchaPopcorn", 1);
        scoreDictionary.Add("StrawberryPopcorn", 1);
        scoreDictionary.Add("HoneyPopcorn", 1);
        spawnManager.enabled = false;

        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            scoreHash = new Hashtable();
            PhotonNetwork.LocalPlayer.SetCustomProperties(scoreHash);
        }
       
        GameStart();
        countDownPanel.SetActive(false);
    }

    public void updateDatabase() {
        DatabaseScript database = new DatabaseScript();
        int prevHighest = DatabaseScript.playerScore;
        if (score > prevHighest)
        {
            database.updateUserScoreToDatabase(score);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If the game starts and didn't reach the time limit, keep processing touches
        if (startSpawn && timeLeft>=0)
        {
            ProcessTouches();
            timeLeft -= Time.deltaTime;
        }
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
                CloudAnchorText.enabled = false;
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

    void GameStart()
    {
        CreatePopcornMachine();
    }

    void StartSpawnPopcorns()
    {
        startSpawn = true;
        countDownPanel.SetActive(false);
        //startSpawn = true;
        spawnManager.enabled = true;
    }

    void StartBonusRound()
    {
        spawnManager.BonusRound = true;
        GetComponent<BonusRound>().enabled = true;
    }

    void GameOver()
    {
        //Destroy all the popcorns and popcorn machine, show final score, check if user clicks restart
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
        }
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("GameOverScene");
        /*
        GameOverAnimator.SetBool("IsGameOver", true);
        finalScoreText.text = "Score: " + score;
        if (CrossPlatformInputManager.GetButtonDown("Restart"))
        {
            SceneManager.LoadScene("MainScene");
            GameOverAnimator.SetBool("IsGameOver", false);
        }
        */
    }

    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount < 1 ||
            (touch = Input.GetTouch(Input.touchCount - 1)).phase != TouchPhase.Began)
        {
            return;
        }
        //Debug.Log("yes We have a touch");
       
        RaycastHit rayHit;
        Ray ray = firstPersonCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
       
        if (Physics.Raycast(ray, out rayHit))
        {
            //Debug.Log("I meet a collider!");
            //Debug.Log("This collider is a" + rayHit.collider.gameObject.tag);
            //DebugText.text= ("This collider is a" + rayHit.collider.gameObject.tag);
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
            else if(scoreToAdd==0)
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
                if (PhotonNetwork.IsMasterClient)
                {
                    mainSceneLatency = new Dictionary<string, string>();
                    pingTime = DateTime.UtcNow;
                    Debug.Log("master time set");
                }
                //Send the message to other players to destroy the popcorn in their local copy of the game
                PhotonView _photonView = itemToCollect.GetComponent<PhotonView>();
                _photonView.RPC("DestroyPopcorn", RpcTarget.Others, _photonView.ViewID);
            }
            
        }
    }

    void DisableBonusRoundPanel()
    {
        bonusRoundInformPanelGameObject.SetActive(false);
    }

    [PunRPC]
    void IncreaseReadyPlayers()
    {
        readyPlayers++;
    }

    [PunRPC]
    void MasterReact(string ID)
    {
        Debug.Log("master reacting!");
        latency = (DateTime.UtcNow - pingTime).TotalMilliseconds / 2;
        mainSceneLatency.Add(ID, latency.ToString());
        foreach (KeyValuePair<string, string> kvp in mainSceneLatency)
        {
            DebugText.text ="key=" + kvp.Key + " | value=" + kvp.Value;
            Debug.Log("key=" + kvp.Key + " | value=" + kvp.Value);
        }
    }
}