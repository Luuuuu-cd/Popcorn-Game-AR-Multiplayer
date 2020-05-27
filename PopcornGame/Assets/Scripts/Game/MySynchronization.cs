using UnityEngine;
using Photon.Pun;

//This script controls the synchronization between popcorn positions
public class MySynchronization : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosition;
    Quaternion networkedRotation;

    private float distance;
    private float angle;
    private GameObject popcornMachineGameObject;

    public bool synchronizeVelocity = true;
    public bool synchronizeAngularVelocity = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();

        popcornMachineGameObject = GameObject.FindGameObjectWithTag("popcornMachine");
    }

    private void FixedUpdate()
    {

        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Then, photonView is mine and I am the one who controls this popcorn.
            //Should send position, velocity etc. data to the other players
            stream.SendNext(rb.position - popcornMachineGameObject.transform.position);
            stream.SendNext(rb.rotation);

            if (synchronizeVelocity)
            {
                stream.SendNext(rb.velocity);
            }

            if (synchronizeAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {

            //Called on my popcorn gameobject that exists in remote player's game

            networkedPosition = (Vector3)stream.ReceiveNext() + popcornMachineGameObject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();


            if (synchronizeVelocity || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if (synchronizeVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();

                    networkedPosition += rb.velocity * lag;

                    distance = Vector3.Distance(rb.position, networkedPosition);
                }

                if (synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();

                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkedRotation;

                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }
            }
        }
    }
}
