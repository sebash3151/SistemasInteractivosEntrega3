using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static int score = 0;
    private int actualHighScore;
    [SerializeField] Text scoretext;
    [SerializeField] Text bigOtext;

    void Start()
    {
        score = 0;
        actualHighScore = PlayerPrefs.GetInt("highScore");
    }

    void Update()
    {
        scoretext.text = PlayerPrefs.GetInt("highScore").ToString();
        bigOtext.text = score.ToString();

        if(score > actualHighScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            actualHighScore = score;
        }
    }
}
