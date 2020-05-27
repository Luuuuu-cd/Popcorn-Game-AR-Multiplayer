using UnityEngine;
using Photon.Pun;


//This script control the behavior of popcorns
public class PopcornController : MonoBehaviour
{
    private GameObject ARCoreDevice;
    private Vector3 devicePos;
    private Vector3 popcornPos;
    private Behaviour halo;
    private PhotonView _photonView;


    // Start is called before the first frame update
    void Start()
    {
        ARCoreDevice = GameObject.FindGameObjectWithTag("ARCoreDevice");
        _photonView = GetComponent<PhotonView>();

        //When popcorn is created, disable halo
        halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
    }


    void Update()
    {
        devicePos = ARCoreDevice.transform.position;
        popcornPos = gameObject.transform.position;

        //If distance between popcorn position and device position is smaller than 1, enable halo
        if (((popcornPos - devicePos).magnitude) < 1.6f)
        {
            halo.enabled = true;
        }
        else
        {
            halo.enabled = false;
        } 
    }

    [PunRPC]
    public void DestroyPopcorn(int viewID)
    {
        Debug.Log("Destroying popcorn!");
        Destroy(PhotonView.Find(viewID).gameObject);
        PhotonView _masterPhotonView = GameObject.Find("GameManager").GetComponent<PhotonView>();
        _masterPhotonView.RPC("MasterReact", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
    }
}
