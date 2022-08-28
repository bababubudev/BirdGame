using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    private bool isPaused;
    private bool isNotMenu;
    private bool toggle;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject modifyMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject resetHighscore;

    [Space, Space]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreGameOver;
    [SerializeField] private TMP_Text highscoreText;

    private TMP_Text highscoreView;
    private TMP_Text resetScoreText;

    private Color alphaColor;


    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        isNotMenu = !(_currentGameState == GameState.MainMenu || _currentGameState == GameState.GameOver);
        mainMenu.SetActive(_currentGameState == GameState.MainMenu);
        pauseMenu.SetActive(_currentGameState == GameState.Pause);
        gameoverMenu.SetActive(_currentGameState == GameState.GameOver);

        if (mainMenu.activeSelf)
        {
            optionMenu.SetActive(false);
            modifyMenu.SetActive(false);
        }
    }
    #endregion

    private void Start()
    {
        var resetTextGO = resetHighscore.transform.GetChild(0);
        var highscoreTextGO = resetHighscore.transform.GetChild(1);

        highscoreTextGO.TryGetComponent(out highscoreView);
        resetTextGO.TryGetComponent(out resetScoreText);

        alphaColor = highscoreView.color;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isNotMenu)
        {
            scorePanel.SetActive(true);
            GameState changedState = isPaused ? GameState.Pause : GameState.Gameplay;
            GameManager.Instance.SetState(changedState);
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            scorePanel.SetActive(false);
        }

        int highScore = PlayerPrefs.GetInt("Highscore");
        scoreText.text = $"Score: {GameManager.currentScore}";
        highscoreText.text = $"HighScore: {highScore}";
        highscoreView.text = highScore.ToString();
        if (highScore == 0)
        {
            alphaColor.a = 0.4f;
            resetHighscore.GetComponent<Button>().interactable = false;
        }
        else
        {
            alphaColor.a = 1f;
            resetHighscore.GetComponent<Button>().interactable = true;
        }

        highscoreView.color = alphaColor;
        resetScoreText.color = alphaColor;
        scoreGameOver.text = GameManager.currentScore.ToString();
    }

    public void PlayGame()
    {
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    public void ModifyGameMenu()
    {
        //First time its true.
        toggle = !toggle;
        mainMenu.SetActive(!toggle);
        modifyMenu.SetActive(toggle);
    }

    public void OptionMenu()
    {
        //First time its true.
        toggle = !toggle;
        mainMenu.SetActive(!toggle);
        optionMenu.SetActive(toggle);
    }

    public void ResumeGame()
    {
        isPaused = false;
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    public void BackToMenu()
    {
        GameManager.Instance.SetState(GameState.MainMenu);
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("Highscore", 0);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Quit");
    }
}
