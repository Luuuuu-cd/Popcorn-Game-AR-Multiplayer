using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.UI;
using System.Threading; 


public class ProfileLauncher : MonoBehaviour
{
    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text scoreText; 

    void Start()
    {
        titleText.text = DatabaseScript.displayName + "!\n" + "Your highest Score is: ";
        scoreText.text = DatabaseScript.playerScore.ToString();
    }

    //public void userProfileButtonClicked() {
    //    GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
    //    SceneManager.LoadScene("UserPersonalAccountPage");
    //}

    //public void backButtonClickedFromProfilePage() {
    //    GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
    //    SceneManager.LoadScene("StartScene");
   // }

    private void Update()
    {
        if(CrossPlatformInputManager.GetButtonDown("PopcornChoice"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("PopcornChoice");
        }

        if (CrossPlatformInputManager.GetButtonDown("ProfileBackButton"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("StartScene");
        }
    }

}
