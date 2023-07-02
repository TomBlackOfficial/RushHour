using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    private int currentScore;
    private int highScore;

    private void Awake()
    {
        _instance = this;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public void Die()
    {
        if (PlayerPrefs.HasKey("highScore"))
        {
            if (currentScore > PlayerPrefs.GetInt("highScore"))
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("highScore", highScore);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("highScore", highScore);
                PlayerPrefs.Save();
            }
        }
    }
}
