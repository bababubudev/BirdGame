using UnityEngine;

public class AudioEventHandler : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip deathMusic;

    private int _prevScore;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        switch (_currentGameState)
        {
            case GameState.Gameplay:
                AudioManager.Instance.PlayMusic(gameMusic, 0.5f);
                break;
            case GameState.Menu:
                AudioManager.Instance.PlayMusic(menuMusic);
                break;
            case GameState.GameOver:
                AudioManager.Instance.PlayMusic(deathMusic, 2f);
                AudioManager.Instance.PlaySound("Death");
                break;
        }
    }

    private void Update()
    {
        if (_prevScore != GameManager.currentScore)
        {
            _prevScore = GameManager.currentScore;
            AudioManager.Instance.PlaySound("Point");
        }
    }
}
