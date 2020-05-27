using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;

//This script controls cloud anchor
public class CloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject HostedPointPrefab;
    [SerializeField]
    private GameObject ResolvedPointPrefab;
    [SerializeField]
    private ARReferencePointManager ReferencePointManager;
    [SerializeField]
    private ARRaycastManager RaycastManager;
    [SerializeField]
    private GameObject OutputPanel;
    [SerializeField]
    private Text OutputText;
    private Pose placePos;
    private GameObject popcornMachine;
    private PhotonView _photonView;
    private enum AppMode
    {
        // Wait for user to tap screen to begin hosting a point.
        ReadyToHostCloudReferencePoint,

        // Poll hosted point state until it is ready to use.
        WaitingForHostedReferencePoint,

        // Wait for user to begin resolving the point.
        ReadyToResolveCloudReferencePoint,

        // Poll resolving point state until it is ready to use.
        WaitingForResolvedReferencePoint,
    }
    private AppMode m_AppMode = AppMode.ReadyToHostCloudReferencePoint;
    private ARCloudReferencePoint m_CloudReferencePoint;
    private string m_CloudReferenceId;


    public bool isPlaced = false;

    void Start()
    {
        //If is single player mode, disable cloud anchor service
        if(StartSceneLauncher._instance.singlePlayerMode)
        {
            OutputPanel.SetActive(false);
            enabled = false;
        }
        popcornMachine = GameObject.FindGameObjectWithTag("popcornMachine");
        _photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        //When the master client place the popcorn machine in their local copy, isPlaced=true
        if (m_AppMode == AppMode.ReadyToHostCloudReferencePoint && isPlaced)
        {
            placePos = new Pose(popcornMachine.transform.position, Quaternion.identity);
            OutputText.text = m_AppMode.ToString();
            ARReferencePoint referencePoint =
                        ReferencePointManager.AddReferencePoint(
                           placePos);

            // Create Cloud Reference Point.
            m_CloudReferencePoint =
                ReferencePointManager.AddCloudReferencePoint(
                    referencePoint);
            if (m_CloudReferencePoint == null)
            {
                OutputText.text = "Create Failed!";
                return;
            }

            // Wait for the reference point to be ready.
            m_AppMode = AppMode.WaitingForHostedReferencePoint;

        }

        else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
        {
           

            CloudReferenceState cloudReferenceState =
                m_CloudReferencePoint.cloudReferenceState;

            if (cloudReferenceState == CloudReferenceState.Success)
            {
                //Instantiate an empty game object to indicate the cloud anchor is placed at the position, can be replaced with other game object for debugging
                OutputText.text = "Success! Click Ready when you are ready";
                GameObject cloudAnchor = Instantiate(
                                             HostedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);

                //Popcorn machine will follow the transform of cloud anchor
                popcornMachine.transform.position = cloudAnchor.transform.position;
                popcornMachine.transform.SetParent(m_CloudReferencePoint.transform);

                //Get the cloud anchor reference Id and send this ID to other clients
                m_CloudReferenceId = m_CloudReferencePoint.cloudReferenceId;
                m_CloudReferencePoint = null;
                _photonView.RPC("setCloudReferenceId", RpcTarget.Others, m_CloudReferenceId);
            }
            else
            {
                OutputText.text = /*m_AppMode.ToString() +*/" Please wait for a few seconds..." + " - " + cloudReferenceState.ToString();
            }
        }

        else if (m_AppMode == AppMode.ReadyToResolveCloudReferencePoint)
        {
            OutputText.text = m_CloudReferenceId;
            {
                m_CloudReferencePoint =
                    ReferencePointManager.ResolveCloudReferenceId(
                        m_CloudReferenceId);
                if (m_CloudReferencePoint == null)
                {
                    OutputText.text = "Resolve Failed!";
                    m_CloudReferenceId = string.Empty;
                    return;
                }

                m_CloudReferenceId = string.Empty;

                // Wait for the reference point to be ready.
                m_AppMode = AppMode.WaitingForResolvedReferencePoint;
            }
        }

        else if (m_AppMode == AppMode.WaitingForResolvedReferencePoint)
        {
            CloudReferenceState cloudReferenceState =
                m_CloudReferencePoint.cloudReferenceState;
            if (cloudReferenceState == CloudReferenceState.Success)
            {
                OutputText.text = "Success! Click Ready when you are ready";
                GameObject cloudAnchor = Instantiate(
                                             ResolvedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);
                popcornMachine.transform.position = cloudAnchor.transform.position;
                popcornMachine.transform.SetParent(m_CloudReferencePoint.transform);
                m_CloudReferencePoint = null;
            }
            else
            {
                OutputText.text = /*m_AppMode.ToString()+*/ " Please wait for a few seconds..." + " - " + cloudReferenceState.ToString();
            }
        }
    }



    [PunRPC]
    void setCloudReferenceId(string id)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            m_CloudReferenceId = string.Empty;
            m_CloudReferenceId = id;
            m_AppMode = AppMode.ReadyToResolveCloudReferencePoint;
        }
    }
}
