using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private Rect boundary;

    private bool jumpDesired;
    private bool isColliding;

    private bool wasGameOver;
    private bool _passedPipe;
    private bool PassedPipe
    {
        get => _passedPipe;
        set
        {
            if (!_passedPipe && value) GameManager.currentScore++;
            _passedPipe = value;
        }
    }

    private bool doneOnce;

    [SerializeField] private float gravity = 1;
    [SerializeField] private float jumpHeight = 1;

    private Vector3 velocity;
    private Vector3 positionFlat;
    private Vector3 resetPosition;

    private SpriteRenderer birdSprite;
    [SerializeField] private LayerMask collisionMask;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        birdSprite = GetComponent<SpriteRenderer>();
        resetPosition = transform.position;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        enabled = !(_currentGameState == GameState.Pause);
        birdSprite.enabled = _currentGameState != GameState.Menu;
        if (_currentGameState == GameState.Menu) transform.position = resetPosition;
        if (wasGameOver && _currentGameState != GameState.GameOver) transform.position = resetPosition;
        wasGameOver = _currentGameState == GameState.GameOver;
        if (wasGameOver) GameManager.currentScore = 0;
        doneOnce = false;
    }
    #endregion

    private void Start()
    {
        boundary = GameManager.Instance.boundary;
    }

    private void Update()
    {
        isColliding = Physics2D.CircleCast(transform.position, 0.5f, Vector2.right, 0f, collisionMask);
        PassedPipe = Physics2D.Raycast(transform.position - Vector3.right, Vector3.down, boundary.height, collisionMask);

        if (isColliding) GameManager.Instance.SetState(GameState.GameOver);

        jumpDesired |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);
        if (GameManager.Instance.CurrentGameState == GameState.GameOver)
        {
            //Animation
            if (transform.position.y > -boundary.height * 0.5f)
            {
                if (!doneOnce)
                {
                    doneOnce = true;
                    velocity = Vector3.zero;

                    float jumpSpeed = Mathf.Sqrt(-2f * -1f * 200f);
                    velocity += Vector3.up * jumpSpeed;
                }
                else
                {
                    velocity.y -= gravity;
                }

                transform.position += velocity * Time.unscaledDeltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (jumpDesired)
        {
            jumpDesired = false;
            velocity = Vector3.zero;

            float jumpSpeed = Mathf.Sqrt(-2f * -gravity * jumpHeight);
            velocity += Vector3.up * jumpSpeed;
        }
        else
        {
            velocity.y -= gravity;
        }

        transform.position += velocity * Time.deltaTime;

        positionFlat = transform.position;

        if (!boundary.Contains(positionFlat))
        {
            velocity = Vector3.zero;
            positionFlat.y = positionFlat.y > (boundary.height * 0.5f) ? boundary.height * 0.5f : -boundary.height * 0.5f;
        }

        transform.position = positionFlat;
    }
}
