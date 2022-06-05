using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null");
            }
            return _instance;
        }
    }
    
    private Camera cam;  
    [SerializeField] private GameState startingState;
    [HideInInspector] public Vector3 bottomLeft;
    [HideInInspector] public Rect boundary;

    public GameState CurrentGameState { get; private set; }
    public delegate void StateChangeHandler(GameState _currentGameState);
    public event StateChangeHandler OnGameStateChanged;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        SetState(startingState);

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
}
