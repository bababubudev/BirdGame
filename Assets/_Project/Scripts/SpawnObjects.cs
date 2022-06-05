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
    [SerializeField] private float distanceToPipe = 0.15f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float spawnTime = 1f;

    [SerializeField] private PipeObj pipeObject;

    #region SubscribeToEvent
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _currentGameState)
    {
        if (_currentGameState == GameState.Menu)
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
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    #endregion

    private void Start()
    {
        boundary = GameManager.Instance.boundary;
        bottomLeft = GameManager.Instance.bottomLeft;

        SetBoundaries();
        StartCoroutine(Spawner());
    }

    private void Update()
    {
        Time.timeScale = GameManager.Instance.CurrentGameState == GameState.Gameplay ? 1 : 0;
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            SpawnPipes();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SpawnPipes()
    {
        Quaternion flipped = Quaternion.Euler(0f, 0f, 180f);
        PipeObj spawnedPipeBottom = Instantiate(pipeObject, boundaryBox.bottomRight + (Vector2.right * 2f), Quaternion.identity, transform);
        PipeObj spawnedPipeTop = Instantiate(pipeObject, boundaryBox.topRight + (Vector2.right * 2f), flipped, transform);
        spawnedPipeBottom.SetSpeed(moveSpeed);
        spawnedPipeTop.SetSpeed(moveSpeed);

        SetPipeLengths(spawnedPipeTop, spawnedPipeBottom, distanceToPipe);
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
        float changeAmount = 0;

        if (totalScaleY > boundary.height)
        {
            changeAmount = totalScaleY - boundary.height;
            float randomTaker = Random.Range(0.2f, 0.6f);
            //print($"Random: {randomTaker}, From: {1 - randomTaker}");

            scaleTop.y -= changeAmount * randomTaker;
            scaleBottom.y -= changeAmount * (1.0f - randomTaker);
        }

        pipeBottom.tubeTransform.localScale = scaleBottom;
        pipeTop.tubeTransform.localScale = scaleTop;

        //print($"Differ: {changeAmount}, ScaleY: {scaleBottom.y}, ScaleYTop: {scaleTop.y}, TotalScale: {totalScaleY}, Height: {boundary.height}");
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
