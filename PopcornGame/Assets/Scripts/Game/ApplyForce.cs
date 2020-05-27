using UnityEngine;

//This script is to apply a random and continuous force to popcorns when they are created
public class ApplyForce : MonoBehaviour
{
    [SerializeField]
    private GameObject ARCoreDevice;

    private Rigidbody rb;
    private Vector3 devicePos;
    private Vector3 popcornPos;

    private float x=0;
    private float y =0;
    private float z =0;


    void Start()
    {
        ARCoreDevice = GameObject.FindGameObjectWithTag("ARCoreDevice");
        //Destroy the popcorn if it exists more than 10 seconds
        Destroy(gameObject, 10);
        rb = gameObject.GetComponent<Rigidbody>();

        //Generate random force in x, y, z directions
        while(x < 3f && x > -3f)
        {
            x = Random.Range(-6.0f, 6.0f);
        }
        while (z < 3f && z > -3f)
        {
            z = Random.Range(-6.0f, 6.0f);
        }
        y = Random.Range(6f, 9f);
  
        Vector3 force = new Vector3(x, y, z);
        rb.AddForce(force,ForceMode.Impulse);
    }


    void Update()
    {
        //Add a lower gravity to popcorns so their motion is slower (easier to catch)
        if (GameItemManager._instance.isFanOn) {
            EnableFan();
        }
        else
        {
            rb.AddForce(Physics.gravity * 0.3f);
        }

        //Destroy the popcorn after it landed
        if (transform.position.y < -1)
        {
            Destroy(gameObject);
        }
    }

    void DisableFan()
    {
        GameItemManager._instance.isFanOn = false;
    }

    void EnableFan()
    {
        Behaviour halo = (Behaviour)GetComponent("Halo");
        if (halo.enabled==true)
        {
            if (gameObject.tag == "RegularPopcorn")
            {
                transform.position = (Vector3.MoveTowards(rb.transform.position, Camera.main.transform.position, (float)0.03));
            }
            else
            {
                rb.AddForce(Physics.gravity * 0.3f);
            }
        }
        else
        {
            rb.AddForce(Physics.gravity * 0.3f);
        }
        devicePos = ARCoreDevice.transform.position;
        popcornPos = gameObject.transform.position;

        if (((popcornPos - devicePos).magnitude) < 0.3f)
        {
            int score;
            if (GameManager._instance.scoreDictionary.TryGetValue("RegularPopcorn", out score))
            {
                GameManager._instance.CollectPopcorn(score, gameObject);
            }
        }
        Invoke("DisableFan", 5);
    }

}
