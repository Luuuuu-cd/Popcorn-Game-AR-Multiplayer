using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PopcornChoice : MonoBehaviour
{
    [SerializeField]
    private Text popcornChoiceText;

    public int popcornChoice = 0;

    public static PopcornChoice _instance;

    void Start()
    {
        _instance = this;
    }

    void Update()
    {
        if(CrossPlatformInputManager.GetButtonDown("Normal"))
        {
            popcornChoice = 1;
            popcornChoiceText.text = "You select Normal Popcorn!";
        }
        if (CrossPlatformInputManager.GetButtonDown("Chocolate"))
        {
            popcornChoice = 2;
            popcornChoiceText.text = "You select Chocolate Popcorn!";
        }
        if (CrossPlatformInputManager.GetButtonDown("Matcha"))
        {
            popcornChoice = 3;
            popcornChoiceText.text = "You select Matcha Popcorn!";
        }
        if (CrossPlatformInputManager.GetButtonDown("Strawberry"))
        {
            popcornChoice = 4;
            popcornChoiceText.text = "You select Strawberry Popcorn!";
        }
        if (CrossPlatformInputManager.GetButtonDown("Honey"))
        {
            popcornChoice = 5;
            popcornChoiceText.text = "You select Honey Popcorn!";
        }
        if(CrossPlatformInputManager.GetButtonDown("PopcornChoiceBack"))
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
            SceneManager.LoadScene("UserPersonalAccountPage");
        }
    }
}
