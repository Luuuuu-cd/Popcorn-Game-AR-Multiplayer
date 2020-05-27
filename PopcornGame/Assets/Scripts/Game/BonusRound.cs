using UnityEngine;
using UnityEngine.UI;

public class BonusRound : MonoBehaviour
{
    //This scripts controls the bonus round, a number represents a popcorn choice, RegularPopcorn:1; ChocolatePopcorn:2; MatchaPopcorn:3;StrawberryPopcorn:4;HoneyPopcorn:5
    private int foodType;

    [SerializeField]
    private GameObject[] popcornImages;

    [SerializeField]
    private Text instrutionText;

    [SerializeField]
    private GameObject panel;


    void OnEnable()
    {
        //If the player choosed popcorn before, use their choice, else, generate a random choice
        try
        {
            foodType = PopcornChoice._instance.popcornChoice;
        }
        catch
        {
            foodType = Random.Range(1, 6);
        }          
    }

    void Update()
    {
        instrutionText.text= "Collect This!!!";
        if (foodType==1)
        {
            GameObject image = Instantiate(popcornImages[0]);
            image.transform.SetParent(panel.transform);
            GetComponent<GameManager>().scoreDictionary["RegularPopcorn"] = 8;
        }
        else if(foodType==2)
        {
            GameObject image = Instantiate(popcornImages[1]);
            image.transform.SetParent(panel.transform);
            GetComponent<GameManager>().scoreDictionary["ChocolatePopcorn"] = 8;
        }
        else if(foodType==3)
        {
            GameObject image = Instantiate(popcornImages[2]);
            image.transform.SetParent(panel.transform);
            GetComponent<GameManager>().scoreDictionary["MatchaPopcorn"] = 8;
        }
        else if (foodType == 4)
        {
            GameObject image = Instantiate(popcornImages[3]);
            image.transform.SetParent(panel.transform);
            GetComponent<GameManager>().scoreDictionary["StrawberryPopcorn"] = 8;
        }
        else if (foodType == 5)
        {
            GameObject image = Instantiate(popcornImages[4]);
            image.transform.SetParent(panel.transform);
            GetComponent<GameManager>().scoreDictionary["HoneyPopcorn"] = 8;
        }
    }
}
