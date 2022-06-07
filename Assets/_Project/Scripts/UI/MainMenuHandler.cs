using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    private bool isPaused;
    private bool isNotMenu;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameoverMenu;

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

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
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
            GameState changedState = isPaused ? GameState.Pause : GameState.Gameplay;
            GameManager.Instance.SetState(changedState);
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
        }
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
