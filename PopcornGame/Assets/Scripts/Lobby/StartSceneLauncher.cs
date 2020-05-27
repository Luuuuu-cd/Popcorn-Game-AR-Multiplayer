using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class StartSceneLauncher : MonoBehaviourPunCallbacks
{
    public bool singlePlayerMode;
    public static StartSceneLauncher _instance;

    private void Start()
    {
        _instance = this;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("SinglePlayerButton"))
        {
            singlePlayerMode = true;
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("MainScene");
            
        }
        if (CrossPlatformInputManager.GetButtonDown("MultiPlayerButton"))
        {
            singlePlayerMode = false;
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("MultiPlayerStartScene");
        }

        if(CrossPlatformInputManager.GetButtonDown("ProfileButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            if (GoogleSignInDemo._instance.isGuest)
            {
                SceneManager.LoadScene("LoginScreen");
            }
            else
            {
                SceneManager.LoadScene("UserPersonalAccountPage");
            }
            
        }

    }
}
