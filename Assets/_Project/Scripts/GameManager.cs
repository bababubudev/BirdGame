using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    string goName = typeof(AudioManager).Name;
                    GameObject go = GameObject.Find(goName);

                    if (go == null)
                    {
                        go = new GameObject
                        {
                            name = goName
                        };
                    }

                    _instance = go.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    public static int currentScore;
    public GameState CurrentGameState { get; private set; }
    public delegate void StateChangeHandler(GameState _currentGameState);
    public event StateChangeHandler OnGameStateChanged;

    private Camera cam;
    [HideInInspector] public Vector3 bottomLeft;
    [HideInInspector] public Rect boundary;

    private void Start()
    {
        cam = Camera.main;
        OnGameStateChanged?.Invoke(GameState.Menu);

        float vertical = cam.orthographicSize;
        float horizontal = vertical * cam.aspect;

        bottomLeft = cam.transform.position - (Vector3.right * horizontal) - (Vector3.up * vertical);
        boundary = new Rect(bottomLeft.x, bottomLeft.y, horizontal * 2f, vertical * 2f);
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
    Menu,
    Gameplay,
    Pause,
    GameOver
}
