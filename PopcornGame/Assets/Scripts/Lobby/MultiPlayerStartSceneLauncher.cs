using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MultiPlayerStartSceneLauncher : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("CreateButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            CreateRoom();
        }
        if (CrossPlatformInputManager.GetButtonDown("JoinButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            JoinRoom();
        }
        if (CrossPlatformInputManager.GetButtonDown("BackButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("StartScene");
        }

    }

    public void CreateRoom()
    {
        SceneManager.LoadScene("Create Room");
    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("Join Room");
    }
}
