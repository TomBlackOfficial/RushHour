using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField] private TextMeshProUGUI firstName;
    [SerializeField] private TextMeshProUGUI secondName;
    [SerializeField] private TextMeshProUGUI thirdName;
    [SerializeField] private TextMeshProUGUI forthName;
    [SerializeField] private TextMeshProUGUI fifthName;

    [SerializeField] private TextMeshProUGUI firstScore;
    [SerializeField] private TextMeshProUGUI secondScore;
    [SerializeField] private TextMeshProUGUI thirdScore;
    [SerializeField] private TextMeshProUGUI forthScore;
    [SerializeField] private TextMeshProUGUI fifthScore;

    [SerializeField] private TMP_InputField usernameInput;

    public enum GameStates
    {
        Menu,
        Playing,
        Paused,
        Dead
    }

    public GameStates currentState;

    public GameObject menuScreen, deathScreen, pausedScreen, rankScreen;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int currentScore;
    private int highScore;

    private HighScoreList highScores = null;


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
        StartCoroutine(GetTopScores());
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString("N0");
    }

    public IEnumerator GetTopScores()
    {
        var request = new UnityWebRequest("https://rushhour-1-n7509643.deta.app/", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        highScores = JsonUtility.FromJson<HighScoreList>(request.downloadHandler.text);
    }

    public void UploadScore()
    {
        if (usernameInput.text == null || usernameInput.text == "" || usernameInput.text == " ")
                return;

        var request = new UnityWebRequest("https://rushhour-1-n7509643.deta.app/?username=" + usernameInput.text + "&score=" + currentScore, "POST");
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

    public void OpenRankScreen()
    {
        StartCoroutine(GetTopScores());
        rankScreen.SetActive(true);

        var test = highScores.scores;
        
        firstName.text = test[0].username;
        firstScore.text = test[0].score.ToString("N0");
        secondName.text = test[1].username;
        secondScore.text = test[1].score.ToString("N0");
        thirdName.text = test[2].username;
        thirdScore.text = test[2].score.ToString("N0");
        forthName.text = test[3].username;
        forthScore.text = test[3].score.ToString("N0");
        fifthName.text = test[4].username;
        fifthScore.text = test[4].score.ToString("N0");
    }

    public void CloseRankScreen()
    {
        rankScreen.SetActive(false);
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
        
        //SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }
}
