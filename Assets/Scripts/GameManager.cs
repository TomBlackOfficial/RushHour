using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentScore;
    private int highScore;

    [Serializable]
    public class HighScore
    {
        public string username;
        public int score;
    }

    [Serializable]
    public class HighScoreList
    {
        public List<HighScore> scores;
    }

    private void Awake()
    {
        _instance = this;
        scoreText.text = currentScore.ToString("N0");
        GetTopScores();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString("N0");
    }
    
    public HighScoreList GetTopScores()
    {
        var request = new UnityWebRequest("https://rushhour-1-n7509643.deta.app/", "GET"); 
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
        
        return JsonUtility.FromJson<HighScoreList>(request.downloadHandler.text);
    }

    private void UploadScore(string username)
    {
        var request = new UnityWebRequest("https://rushhour-1-n7509643.deta.app/?username=" + username + "&score=" + currentScore, "POST");
        request.SendWebRequest();
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
        
        UploadScore("Test2");
        
        //SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }
}
