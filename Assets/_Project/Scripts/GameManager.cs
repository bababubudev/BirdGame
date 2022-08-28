using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting;
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
            }

            return _instance;
        }
    }

    private void OnDestroy()
    {
        _applicationIsQuitting = true;
    }

    public static int currentScore;

    public GameState CurrentGameState { get; private set; }
    public delegate void StateChangeHandler(GameState _currentGameState);
    public event StateChangeHandler OnGameStateChanged;

    
    private Camera cam;
    [HideInInspector] public Vector3 bottomLeft;
    [HideInInspector] public Rect boundary;

    private void Awake()
    {
        cam = Camera.main;

        float vertical = cam.orthographicSize;
        float horizontal = vertical * cam.aspect;

        bottomLeft = cam.transform.position - (Vector3.right * horizontal) - (Vector3.up * vertical);
        boundary = new Rect(bottomLeft.x, bottomLeft.y, horizontal * 2f, vertical * 2f);
    }

    private void Start()
    {
        OnGameStateChanged?.Invoke(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentGameState) return;
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
    GameOver
}
