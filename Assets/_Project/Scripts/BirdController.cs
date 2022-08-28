using System;
using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private Rect boundary;
    private RaycastHit2D isColliding;

    private bool jumpDesired;

    private bool wasGameOver;
    private bool _passedPipe;
    private bool PassedPipe
    {
        get => _passedPipe;
        set
        {
            if (!_passedPipe && value)
            {
                GameManager.currentScore++;
            }

            _passedPipe = value;
        }
    }

    private bool doneOnce;

    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight = 1;

    [Space, Space]
    [SerializeField] private float idlingHeight;
    [SerializeField] private float idleLoopPeriod;

    private Vector3 velocity;
    private Vector3 positionFlat;
    private Vector3 resetPosition = new Vector3(-3f, 0f, 0f);

    [Space, Space]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask scoreMask;
    [SerializeField] private Transform beakTransform;
    [SerializeField] private Transform wingsTransform;

    [Space, Space]
    [SerializeField] private ModifyValues modifiedValues;

    private IEnumerator currentCoroutine;

    private LayerMask collisionlessMask = 0;
    private LayerMask currentMask;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        ValueModification();
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        enabled = !(_currentGameState == GameState.Pause);
        wingsTransform.gameObject.SetActive(_currentGameState is GameState.Gameplay or GameState.Pause);

        if (_currentGameState == GameState.MainMenu)
        {
            transform.position = resetPosition;


            StopAllCoroutines();
            beakTransform.eulerAngles = Vector3.zero;
            wingsTransform.eulerAngles = Vector3.zero;
        }

        if (wasGameOver && _currentGameState != GameState.GameOver)
        {
            GameManager.currentScore = 0;
            transform.position = resetPosition;
        }

        wasGameOver = _currentGameState == GameState.GameOver;

        if (wasGameOver)
        {
            if (PlayerPrefs.GetInt("Highscore") < GameManager.currentScore)
            {
                PlayerPrefs.SetInt("Highscore", GameManager.currentScore);
            }

            StopAllCoroutines();
            beakTransform.eulerAngles = Vector3.zero;
            wingsTransform.eulerAngles = Vector3.zero;
        }

        doneOnce = false;
    }
    #endregion

    private void ValueModification()
    {
        currentMask = collisionMask;
        modifiedValues.GravityScale = gravity;
        modifiedValues.JumpHeight = jumpHeight;
        modifiedValues.DisableCollisions = false;
    }

    private void Modify(string _changeName)
    {
        switch (_changeName)
        {
            case nameof(modifiedValues.GravityScale):
                gravity = modifiedValues.GravityScale;
                break;
            case nameof(modifiedValues.JumpHeight):
                jumpHeight = modifiedValues.JumpHeight;
                break;
            case nameof(modifiedValues.DisableCollisions):
                currentMask = modifiedValues.DisableCollisions ? collisionlessMask : collisionMask;
                break;
        }
    }

    private void Start()
    {
        modifiedValues.OnValueChanged += Modify;
        boundary = GameManager.Instance.boundary;
    }

    private void Update()
    {
        isColliding = Physics2D.CircleCast(transform.position, 0.5f, Vector2.right, 0f, currentMask);
        PassedPipe = Physics2D.Raycast(transform.position - Vector3.right, Vector3.down, boundary.height, scoreMask);

        if (isColliding) GameManager.Instance.SetState(GameState.GameOver);

        jumpDesired |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);

        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.GameOver:
                AnimateDeath();
                break;
            case GameState.MainMenu:
                AnimateIdle();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (jumpDesired)
        {
            Jump();
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

            if (positionFlat.y < boundary.height * 0.5f)
                GameManager.Instance.SetState(GameState.GameOver);
        }

        transform.position = positionFlat;
    }

    private void Jump()
    {
        jumpDesired = false;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine); 

        currentCoroutine = AnimateBeak();
        StartCoroutine(currentCoroutine);
        AudioManager.Instance.PlaySound("Jump");
        velocity = Vector3.zero;


        float jumpSpeed = Mathf.Sqrt(-2f * -gravity * jumpHeight);
        velocity += Vector3.up * jumpSpeed;
    }

    private void AnimateDeath()
    {
        if (transform.position.y > -boundary.height * 0.5f)
        {
            if (!doneOnce)
            {
                doneOnce = true;
                velocity = Vector3.zero;

                float jumpSpeed = Mathf.Sqrt(-2f * -1f * 200f);
                velocity += ((Vector3)isColliding.normal * (jumpSpeed * 0.3f)) + (Vector3.up * jumpSpeed);
            }
            else
            {
                velocity.y -= gravity;
            }

            transform.position += velocity * Time.unscaledDeltaTime;
        }
    }

    private void AnimateIdle()
    {
        float cycles = Time.unscaledTime / idleLoopPeriod;
        float rawSineWave = Mathf.Sin(cycles * Mathf.PI * 2);
        float yPos = idlingHeight * rawSineWave;

        float smoothCurve = Mathf.MoveTowards(transform.position.y, yPos, idleLoopPeriod);
        transform.position = new Vector3(transform.position.x, smoothCurve, transform.position.z);
    }

    private IEnumerator AnimateBeak()
    {
        wingsTransform.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float rawSineWave = Mathf.Sin(t * Mathf.PI * 2);
            float rawSineWaveMult = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 4));

            beakTransform.localRotation = Quaternion.Euler(0f, 0f, 45f * rawSineWave);
            wingsTransform.localRotation = Quaternion.Euler(0f, 0f, 90f * rawSineWaveMult);

            yield return new WaitForEndOfFrame();
        }

        wingsTransform.gameObject.SetActive(false);
    }

}
