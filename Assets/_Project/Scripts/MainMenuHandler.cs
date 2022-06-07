using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    private bool isPaused;
    private bool isNotMenu;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TMP_Text scoreText;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        isNotMenu = !(_currentGameState == GameState.Menu || _currentGameState == GameState.GameOver);
        mainMenu.SetActive(_currentGameState == GameState.Menu);
        pauseMenu.SetActive(_currentGameState == GameState.Pause);
        gameoverMenu.SetActive(_currentGameState == GameState.GameOver);
    }
    #endregion

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

        scoreText.text = GameManager.currentScore.ToString();
    }

    public void PlayGame()
    {
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    public void ResumeGame()
    {
        isPaused = false;
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    public void BackToMenu()
    {
        GameManager.Instance.SetState(GameState.Menu);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Quit");
    }
}
