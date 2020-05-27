using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameItemManager : MonoBehaviour
{
    private PhotonView _photonView;
    public string itemInventory;
    private GameObject fanGameObject;
    public GameObject ARCoreDevice;
    public GameObject fanButton;
    public GameObject inkButton;
    public GameObject inkObject;
    public GameObject fanPrefab;
    public static bool isFanOn = false;
    public static GameItemManager _instance;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        ARCoreDevice = GameObject.FindGameObjectWithTag("ARCoreDevice");
        _photonView = GetComponent<PhotonView>();
        inkObject.SetActive(false);
        inkButton.SetActive(false);
        fanButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.startSpawn==true)
        {
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
                //fanGameObject = Instantiate(fanPrefab, ARCoreDevice.transform.position, Quaternion.identity);
                itemInventory = null;
                isFanOn = true;
                fanButton.SetActive(false);
            }
            if(CrossPlatformInputManager.GetButtonDown("Ink"))
            {
                itemInventory = null;
                inkButton.SetActive(false);
                Debug.Log("InkButtonDown");
                _photonView.RPC("InkIsOn", RpcTarget.Others);
            }
        }
    }


    //public void tellPlayersInkIsOn()
    //{
    //    PhotonView _photonView = new PhotonView();
    //    _photonView.RPC("InkIsOn", RpcTarget.Others, (byte)1);
    //}

    void DisableInk()
    {
        inkObject.SetActive(false);
    }


    [PunRPC]
    public void InkIsOn()
    {
        Debug.Log("InkIsOn");
        inkObject.SetActive(true);
        Invoke("DisabeInk", 3);
    }
}
