using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]
    private Text PlayerNameText;

    [SerializeField]
    private Text PlayerScoreText;

    public void Initialize(string playerName, int score)
    {
        PlayerNameText.text = playerName;
        PlayerScoreText.text = score.ToString();
    }
}
