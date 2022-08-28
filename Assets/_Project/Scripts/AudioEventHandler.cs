using UnityEngine;

public class AudioEventHandler : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip deathMusic;

    private int _prevScore;
    private bool isGameplay;
    private bool triggered = true;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        switch (_currentGameState)
        {
            case GameState.MainMenu:
                isGameplay = triggered = false;
                AudioManager.Instance.PlayMusic(menuMusic);
                break;
            case GameState.GameOver:
                isGameplay = triggered = false;
                AudioManager.Instance.PlayMusic(deathMusic, 2f);
                AudioManager.Instance.PlaySound("Death");
                break;
            case GameState.Gameplay:
            case GameState.Pause:
                isGameplay = true;
                break;
        }
    }

    private void Update()
    {
        if (isGameplay && !triggered)
        {
            triggered = true;
            AudioManager.Instance.PlayMusic(gameMusic, 0.5f);
        }

        if (_prevScore != GameManager.currentScore)
        {
            _prevScore = GameManager.currentScore;
            AudioManager.Instance.PlaySound("Point");
        }
    }
}
