using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject fanSlot;
    [SerializeField]
    private GameObject inkSlot;
    [SerializeField]
    private GameObject fanButton;
    [SerializeField]
    private GameObject inkButton;
    [SerializeField]
    private GameObject inkObject;
    [SerializeField]
    private GameObject fanPrefab;
    [SerializeField]
    private Image fanCD;
    [SerializeField]
    private Image inkCD;
    [SerializeField]
    private Text gameInfoText;

    private PhotonView _photonView;
    private GameObject fanGameObject;
    private GameObject ARCoreDevice;
    private bool isFanReady;
    private bool isInkReady;
    private float maxCoolDownTime = 10f;
    private float fanCoolDown;
    private float inkCoolDown;

    public bool isFanOn = false;
    public static GameItemManager _instance;
    public string itemInventory;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        ARCoreDevice = GameObject.FindGameObjectWithTag("ARCoreDevice");
        itemInventory = null;
        _photonView = GetComponent<PhotonView>();
        inkObject.SetActive(false);
        fanSlot.SetActive(false);
        inkSlot.SetActive(false);
        inkButton.SetActive(false);
        fanButton.SetActive(false);
        fanCD.enabled = false;
        inkCD.enabled = false;
        isFanReady = true;
        isInkReady = true;
        fanCoolDown = 0;
        inkCoolDown = 0;
    }

    void Update()
    {
        if(GameManager._instance.startSpawn==true)
        {
            fanSlot.SetActive(true);
            inkSlot.SetActive(true);
            handleCoolDown();
            if(itemInventory=="Fan")
            {
                fanButton.SetActive(true);
            }
            if(itemInventory=="Ink")
            {
                inkButton.SetActive(true);
            }
            if (CrossPlatformInputManager.GetButtonDown("Fan"))
            {
                if (isFanReady)
                {
                    gameInfoText.text = ("You used fan");
                    if (!StartSceneLauncher._instance.singlePlayerMode)
                    {
                        _photonView.RPC("GameItemInfo", RpcTarget.Others, PhotonNetwork.NickName + " used fan");
                    }
                    itemInventory = null;
                    isFanOn = true;
                    fanButton.SetActive(false);
                    isFanReady = false;
                    fanCoolDown = maxCoolDownTime;
                    fanCD.enabled = true;
                    fanCD.fillAmount = 1;
                }
            }
            if (!StartSceneLauncher._instance.singlePlayerMode)
            {
                if (CrossPlatformInputManager.GetButtonDown("Ink"))
                {
                    if (isInkReady)
                    {
                        gameInfoText.text = ("You used ink");
                        _photonView.RPC("GameItemInfo", RpcTarget.Others, PhotonNetwork.NickName + " used ink");
                        itemInventory = null;
                        inkButton.SetActive(false);
                        _photonView.RPC("InkIsOn", RpcTarget.Others);

                        isInkReady = false;
                        inkCoolDown = maxCoolDownTime;
                        inkCD.enabled = true;
                        inkCD.fillAmount = 1;
                    }
                }  
            }
        }
    }


    void DisableInk()
    {
        inkObject.SetActive(false);
    }

    private void handleCoolDown()
    {
        if (!isFanReady)
        {
            fanCoolDown -= Time.deltaTime;
            fanCD.fillAmount = fanCoolDown / maxCoolDownTime;
            if(fanCoolDown < 0)
            {
                fanCoolDown = 0;
                isFanReady = true;
                fanCD.enabled = false;
            }
        }

        if (!isInkReady)
        {
            inkCoolDown -= Time.deltaTime;
            inkCD.fillAmount = inkCoolDown / maxCoolDownTime;
            if(inkCoolDown < 0)
            {
                inkCoolDown = 0;
                isInkReady = true;
                inkCD.enabled = false;
            }
        }
    }

    [PunRPC]
    public void InkIsOn()
    {
        inkObject.SetActive(true);
        Invoke("DisableInk", 3);
    }

    [PunRPC]
    public void GameItemInfo(string infoContent)
    {
        gameInfoText.text = infoContent;
    }
}
