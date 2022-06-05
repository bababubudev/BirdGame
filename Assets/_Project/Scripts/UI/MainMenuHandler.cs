using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    private bool isPaused;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (GameManager.Instance.CurrentGameState != GameState.Menu)
        {
            GameState changedState = isPaused ? GameState.Pause : GameState.Gameplay;
            GameManager.Instance.SetState(changedState);
            pauseMenu.SetActive(isPaused);
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

    }

    public void PlayGame()
    {
        GameManager.Instance.SetState(GameState.Gameplay);
        mainMenu.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    public void BackToMenu()
    {
        GameManager.Instance.SetState(GameState.Menu);
        mainMenu.SetActive(true);
    }
}
