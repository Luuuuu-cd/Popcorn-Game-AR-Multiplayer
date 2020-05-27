using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//This script allows the popcorn machine move to the position where the player points to in their camera
public class ARPlacementManager : MonoBehaviour
{
    ARRaycastManager m_ARRaycastManager;
    static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit>();

    [SerializeField]
    private Camera aRCamera;
    private GameObject popcornMachine;

    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Start()
    {
        popcornMachine = GameObject.FindGameObjectWithTag("popcornMachine");
    }

    void Update()
    {
        Vector3 centreOfScreen = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = aRCamera.ScreenPointToRay(centreOfScreen);
        if(m_ARRaycastManager.Raycast(ray,raycast_Hits,TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycast_Hits[0].pose;
            Vector3 positionToBePlaced = hitPose.position;
            popcornMachine.transform.position = positionToBePlaced;
        }
    }
}
