using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public enum GameStates
    {
        Menu,
        Playing,
        Paused,
        Dead
    }

    public GameStates currentState;

    public GameObject menuScreen, deathScreen, pausedScreen;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int currentScore;
    private int highScore;


    private float targetTimeScale = 1;

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

        SetGameState(GameStates.Menu);

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentState == GameStates.Menu)
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && currentState != GameStates.Dead && currentState != GameStates.Menu)
        {
            PauseGame();
        }

        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, 4f * Time.deltaTime);
    }

    public void Die()
    {
        SetGameState(GameStates.Dead);

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

        highScoreText.text = PlayerPrefs.GetInt("highScore").ToString("N0");

        SpawnManager._instance.StopSpawning();
        SetGameState(GameStates.Dead);
    }

    public void SpawnVFX(GameObject vfx, Vector3 position, Quaternion rotation)
    {
        Instantiate(vfx, position, rotation);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        SpawnManager._instance.StartSpawning();
        SetGameState(GameStates.Playing);
    }

    public void PauseGame()
    {
        if (currentState == GameStates.Dead || currentState == GameStates.Menu)
            return;

        if (currentState == GameStates.Paused)
        {
            ResumeGame();
            return;
        }

        SetGameState(GameStates.Paused);
    }

    public void ResumeGame() { SetGameState(GameStates.Playing); }

    public void SetGameState(GameStates state)
    {
        currentState = state;
        UpdateScreens();
    }

    private void UpdateScreens()
    {
        if (currentState == GameStates.Playing)
        {
            targetTimeScale = 1;
            Time.timeScale = 1;

            menuScreen.SetActive(false);
            deathScreen.SetActive(false);
            pausedScreen.SetActive(false);
        }
        else if (currentState == GameStates.Paused)
        {
            targetTimeScale = 0;
            Time.timeScale = 0;

            menuScreen.SetActive(false);
            deathScreen.SetActive(false);
            pausedScreen.SetActive(true);
        }
        else if (currentState == GameStates.Dead)
        {
            targetTimeScale = 0;

            menuScreen.SetActive(false);
            deathScreen.SetActive(true);
            pausedScreen.SetActive(false);
        }
        else if (currentState == GameStates.Menu)
        {
            targetTimeScale = 0;

            menuScreen.SetActive(true);
            deathScreen.SetActive(false);
            pausedScreen.SetActive(false);
        }

        UploadScore("Test2");
        
        //SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }
}
