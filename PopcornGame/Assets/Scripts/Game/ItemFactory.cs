using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField]
    GameObject regularPopcorn,chocolatePopcorn,honeyPopcorn,matchaPopcorn,strawberryPopcorn,donut,bananaPeel,fan,inkBottle;

    public GameObject GetPrefabOfType(string type)
    {
        switch (type)
        {
            case ("RegularPopcorn"): return (regularPopcorn);
            case ("ChocolatePopcorn"): return (chocolatePopcorn);
            case ("HoneyPopcorn"): return (honeyPopcorn);
            case ("MatchaPopcorn"): return (matchaPopcorn);
            case ("StrawberryPopcorn"): return (strawberryPopcorn);
            case ("Donut"): return (donut);
            case ("BananaPeel"): return (bananaPeel);
            case ("Fan"): return (fan);
            case ("Ink"): return (inkBottle);
            default: return null;
        }
    }
}
