using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentScore;
    private int highScore;

    private void Awake()
    {
        _instance = this;
        scoreText.text = currentScore.ToString("N0");
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString("N0");
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
        
        //SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }
}
