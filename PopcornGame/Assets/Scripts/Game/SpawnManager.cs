using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

//This script is used to spwan popcorns
public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] popcornPrefabs;

    private GameObject popcornMachineGameObject;
    private bool StartSpawn = false;
    private float timer = 1f;

    public bool BonusRound = false;
    public enum RaiseEventCodes
    {
        SpawnEventCode = 0,
    }

    void Start()
    {
        popcornMachineGameObject = GameObject.FindGameObjectWithTag("popcornMachine");
        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }
            
    }

    void Update()
    {
        //If not single player mode and the client is the master client, start spawn popcorns
        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartSpawn = true;
            }
        }
        //If is single player mode, start spawn popcorns
        else
        {
            StartSpawn = true;
        }
        
        //If there is no popcorn machine object, stop spawn popcorns
        if(popcornMachineGameObject==null)
        {
            StartSpawn = false;
        }

        if(StartSpawn&&!BonusRound)
        {
            //Spawn popcorns in a time interval
            timer -= 1 * Time.deltaTime;
            if (timer < 0)
            {
                int rndi = Random.Range(1, 100);
                if(rndi <= 70)
                {
                    SpawnItem("RegularPopcorn");
                }
                else if(rndi > 70 && rndi <= 80)
                {
                    SpawnItem("Donut");
                }
                else if (rndi > 80 && rndi <= 90)
                {
                    SpawnItem("BananaPeel");
                }
                else if(rndi > 90 && rndi <= 95)
                {
                    SpawnItem("Fan");
                }
                //Only spawn ink object if not single player mode
                else if(!StartSceneLauncher._instance.singlePlayerMode)
                {
                    SpawnItem("Ink");
                }
            }
        }
        //Spawn popcorns with different flavors in bonus round
        else if(StartSpawn && BonusRound)
        {
            timer -= 1 * Time.deltaTime;
            if (timer < 0)
            {
                int rndi = Random.Range(1, 100);
                if (rndi <= 20)
                {
                    SpawnItem("RegularPopcorn");
                }
                else if (rndi > 20 && rndi <= 40)
                {
                    SpawnItem("ChocolatePopcorn");
                }
                else if (rndi > 40 && rndi <= 60)
                {
                    SpawnItem("MatchaPopcorn");
                }
                else if (rndi > 60 && rndi <= 80)
                {
                    SpawnItem("StrawberryPopcorn");
                }
                else
                {
                    SpawnItem("HoneyPopcorn");
                }
            }  
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #region Photon Callback Methods
    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.SpawnEventCode)
        {
            //Clients receive data about popcorn and create the same one in local copy
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            GameObject itemGameObject = Instantiate(GetComponent<ItemFactory>().GetPrefabOfType((string)data[3]), receivedPosition + popcornMachineGameObject.transform.position, receivedRotation);
            itemGameObject.tag = (string)data[3];
            PhotonView _photonView = itemGameObject.GetComponent<PhotonView>();
            _photonView.ViewID = (int)data[2];
        }
    }
    #endregion


    #region Private Methods
    private void SpawnItem(string type)
    {
        PhotonView _photonView=null;
        GameObject itemGameObject=null;

        //Spawn popcorn in local copy of the game, and send the data (popcorn position, popcorn rotation,PhotonView ID) to other clients by rasing an event
        Vector3 instantiatePosition = popcornMachineGameObject.transform.position;
        instantiatePosition = new Vector3(instantiatePosition.x, instantiatePosition.y + 0.8f, instantiatePosition.z);
        itemGameObject = Instantiate(GetComponent<ItemFactory>().GetPrefabOfType(type), instantiatePosition, Quaternion.identity);
        _photonView = itemGameObject.GetComponent<PhotonView>();
        timer = Random.Range(0.3f, 0.8f);
        itemGameObject.tag = type;
        if (!StartSceneLauncher._instance.singlePlayerMode)
        {
            if (PhotonNetwork.AllocateViewID(_photonView))
            {
                object[] data = new object[]
                {
                    itemGameObject.transform.position-popcornMachineGameObject.transform.position, itemGameObject.transform.rotation, _photonView.ViewID,itemGameObject.tag
                };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };
                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };
                //Raise Events!
                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.SpawnEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.Log("Failed to allocate a viewID");
                Destroy(itemGameObject);
            }
        }
    }
    #endregion
}