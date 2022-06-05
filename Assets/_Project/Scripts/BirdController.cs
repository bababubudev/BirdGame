using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private Rect boundary;

    private bool jumpDesired;
    private bool isColliding;

    [SerializeField] private float gravity = 1;
    [SerializeField] private float jumpHeight = 1;

    private Vector3 velocity;
    private Vector3 positionFlat;

    [SerializeField] private LayerMask collisionMask;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        gameObject.SetActive(_currentGameState != GameState.Menu);
        enabled = _currentGameState != GameState.Pause;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    #endregion

    private void Start()
    {
        gameObject.SetActive(GameManager.Instance.CurrentGameState != GameState.Menu);
        boundary = GameManager.Instance.boundary;
    }

    private void Update()
    {
        jumpDesired |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);
        isColliding = Physics2D.OverlapCircle(transform.position, 0.5f, collisionMask);
        print(isColliding);
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
