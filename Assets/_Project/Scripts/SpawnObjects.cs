using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    private BoundingBox boundaryBox;
    private Rect boundary;
    private Vector3 bottomLeft;

    [Range(0, 1f), SerializeField] private float pipeMaxHeight = 0.75f;
    [Range(0, 1f), SerializeField] private float pipeMinHeight = 0.15f;
    [SerializeField] private float minGap = 0.15f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float spawnTime = 1f;

    [SerializeField] private ModifyValues modifiedValues;

    private bool wasGameOver;
    private bool isGaming;

    [SerializeField] private PipeObj pipeObject;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        ValueModification();
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        bool _resetGame = wasGameOver && _currentGameState != GameState.GameOver;
        isGaming = _currentGameState == GameState.Gameplay;

        if (_currentGameState == GameState.MainMenu || _resetGame)
        {
            StopCoroutine(Spawner());
            if (pipeObject != null)
            {
                foreach (PipeObj pole in FindObjectsOfType<PipeObj>())
                {
                    Destroy(pole.gameObject);
                }
            }
        }
        wasGameOver = _currentGameState == GameState.GameOver;

        Time.timeScale = GameManager.Instance.CurrentGameState == GameState.Gameplay ? 1 : 0;
    }
    #endregion

    private void ValueModification()
    {
        modifiedValues.MoveSpeed = moveSpeed;
        modifiedValues.SpawnTime = spawnTime;
        modifiedValues.PipeGap = minGap;
    }

    private void Modify(string _modifiedName)
    {
        switch (_modifiedName)
        {
            case nameof(modifiedValues.MoveSpeed):
                moveSpeed = modifiedValues.MoveSpeed;
                break;
            case nameof(modifiedValues.PipeGap):
                minGap = modifiedValues.PipeGap;
                break;
            case nameof(modifiedValues.SpawnTime):
                spawnTime = modifiedValues.SpawnTime;
                break;
        }
    }

    private void Start()
    {
        modifiedValues.OnValueChanged += Modify;

        boundary = GameManager.Instance.boundary;
        bottomLeft = GameManager.Instance.bottomLeft;

        SetBoundaries();
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            if (isGaming)
            {
                yield return new WaitForSeconds(spawnTime);
                SpawnPipes();
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnPipes()
    {
        Quaternion flipped = Quaternion.Euler(0f, 0f, 180f);
        PipeObj spawnedPipeTop = Instantiate(pipeObject, boundaryBox.topRight + (Vector2.right * 2f), flipped, transform);
        PipeObj spawnedPipeBottom = Instantiate(pipeObject, boundaryBox.bottomRight + (Vector2.right * 2f), Quaternion.identity, transform);
        spawnedPipeBottom.SetSpeed(moveSpeed);
        spawnedPipeTop.SetSpeed(moveSpeed);

        SetPipeLengths(spawnedPipeTop, spawnedPipeBottom, minGap);
    }

    private void SetPipeLengths(PipeObj pipeTop, PipeObj pipeBottom, float distanceBetween)
    {
        float maxHeight = boundary.height * pipeMaxHeight;
        float minHeight = boundary.height * pipeMinHeight;

        Vector3 scaleBottom = pipeBottom.tubeTransform.localScale;
        Vector3 scaleTop = pipeTop.tubeTransform.localScale;

        scaleBottom.y = Random.Range(minHeight, maxHeight);
        scaleTop.y = Random.Range(minHeight, maxHeight);

        float totalScaleY = (scaleTop.y + scaleBottom.y) + distanceBetween;
        if (totalScaleY > boundary.height)
        {
            float changeAmount = totalScaleY - boundary.height;
            float randomTaker = Random.Range(0.2f, 0.6f);
            //print($"Random: {randomTaker}, From: {1 - randomTaker}");

            scaleTop.y -= changeAmount * randomTaker;
            scaleBottom.y -= changeAmount * (1.0f - randomTaker);
        }

        pipeBottom.tubeTransform.localScale = scaleBottom;
        pipeTop.tubeTransform.localScale = scaleTop;

        //print($"ScaleY: {scaleBottom.y}, ScaleYTop: {scaleTop.y}, TotalScale: {totalScaleY}, Height: {boundary.height}");
    }

    private void SetBoundaries()
    {
        boundaryBox.bottomLeft = bottomLeft;
        boundaryBox.bottomRight = bottomLeft + (Vector3.right * boundary.width);
        boundaryBox.topLeft = bottomLeft + (Vector3.up * boundary.height);
        boundaryBox.topRight = bottomLeft + (Vector3.up * boundary.height) + (Vector3.right * boundary.width);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boundary.center, boundary.size);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(boundaryBox.topRight, 0.2f);
        Gizmos.DrawWireSphere(boundaryBox.topLeft, 0.2f);
        Gizmos.DrawWireSphere(boundaryBox.bottomLeft, 0.2f);
        Gizmos.DrawWireSphere(boundaryBox.bottomRight, 0.2f);
    }

    private struct BoundingBox
    {
        public Vector2 topRight, bottomRight;
        public Vector2 topLeft, bottomLeft;
    }
}
