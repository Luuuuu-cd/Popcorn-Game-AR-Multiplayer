using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityStandardAssets.CrossPlatformInput;


//This script is to control the popcorn machine placement proces
public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{
    private ARPlaneManager m_ARPlaneManager;
    private ARPlacementManager m_ARPlacementManager;
    private CloudAnchorManager m_CloudAnchorManager;
    private GameObject popcornMachine;

    #region Private Serializable Fields
    [Tooltip("The button to place the popcorn machine")]
    [SerializeField]
    private GameObject placeButton;

    [Tooltip("The button to click when a player is ready for the game")]
    [SerializeField]
    private GameObject readyButton;

    [Tooltip("The text to display instrucitions and warnings")]
    [SerializeField]
    private Text informUIPanelText;

    [Tooltip("The text to display information during the game process")]
    [SerializeField]
    private Text gameInfoText;

    [Tooltip("The UI panel for instrustions and warnings")]
    [SerializeField]
    private GameObject uI_InformPanelGameObject;

    [Tooltip("The image to represent the centre of the screen")]
    [SerializeField]
    private GameObject raycastCentreImage;

    [Tooltip("The slider to control the size of the popcorn machine")]
    [SerializeField]
    private GameObject scaleSlider;
    #endregion


    #region MonoBehaviour CallBacks
    private void Awake()
    {
        uI_InformPanelGameObject.SetActive(true);
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
        m_CloudAnchorManager = GetComponent<CloudAnchorManager>();
    }

    void Start()
    {
        popcornMachine = GameObject.FindGameObjectWithTag("popcornMachine");

        //If is single player mode or player is the master client, this player should place the popcorn machine
        if(StartSceneLauncher._instance.singlePlayerMode||PhotonNetwork.IsMasterClient)
        {
            placeButton.SetActive(true);
            scaleSlider.SetActive(true);
            readyButton.SetActive(false);
            informUIPanelText.text = "Move phone to detect planes and place the popcorn machine...Please walk around and scan the surroundings for a better experience! For better experience, do scan for more than 15 seconds! ";

        }
        //If the player is not the master client, then this player should wait for master client to place the popcorn machine
        else if (!PhotonNetwork.IsMasterClient)
        {
            m_ARPlaneManager.enabled = false;
            m_ARPlacementManager.enabled = false;
            placeButton.SetActive(false);
            scaleSlider.SetActive(false);
            readyButton.SetActive(true);
            setAllPlanesActiveOrDeactive(false);
            informUIPanelText.text = "Waiting for host to place popcorn machine... Please walk around and scan the surroundings for a better experience! For better experience, do scan for more than 15 seconds! Start In 3 seconds!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Player decide to place the popcorn machine
        if (CrossPlatformInputManager.GetButtonDown("Place"))
        {
            //If the popcorn machine is not placed and the player click place button, inform player to rescan the surface
            if(popcornMachine.transform.position.y > 3)
            {
                gameInfoText.text = "Please rescan the surface";
            }
            else
            {
                DisableARPlacementAndPlaneDetection();
            }

        }
        if (CrossPlatformInputManager.GetButtonDown("Ready"))
        {
            
            uI_InformPanelGameObject.SetActive(false);
            readyButton.SetActive(false);
            scaleSlider.SetActive(false);
            raycastCentreImage.SetActive(false);

            if (StartSceneLauncher._instance.singlePlayerMode)
            {
                GameManager._instance.singlePlayerReady = true;
            }
            //This will send a message to all the clients and increase their local copy of the number of ready players by 1
            else
            {
                PhotonView _photonView = GameManager._instance.GetComponent<PhotonView>();
                _photonView.RPC("IncreaseReadyPlayers", RpcTarget.AllBuffered);
            }
            
            
        }
        if(CrossPlatformInputManager.GetButtonDown("GotIt"))
        {
            DisableInformUIPanel();
        }
    }
    #endregion

    #region Public Methods
    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;
        m_CloudAnchorManager.isPlaced = true;
        setAllPlanesActiveOrDeactive(false);
        placeButton.SetActive(false);
        scaleSlider.SetActive(false);
        readyButton.SetActive(true);
        informUIPanelText.text = "Great! You placed the popcorn machine...Now, click READY when you are ready!";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;
        setAllPlanesActiveOrDeactive(true);
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);
        readyButton.SetActive(false);
        informUIPanelText.text = "Move phone to detect planes and place the popcorn machine";
    }

    public void setAllPlanesActiveOrDeactive(bool value)
    {
        foreach(var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }


    public void DisableInformUIPanel()
    {
        uI_InformPanelGameObject.SetActive(false);
    }
    #endregion
}
