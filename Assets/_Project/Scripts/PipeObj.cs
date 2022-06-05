using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeObj : MonoBehaviour
{
    public Transform tubeTransform;
    public Transform pipeOpening;
    private float speed;

    private void Update()
    {
        pipeOpening.position = tubeTransform.GetChild(0).position;

        Vector2 moveDir = speed * Time.deltaTime * -Vector2.right;
        transform.Translate(moveDir, Space.World);

        if (transform.position.x < -GameManager.Instance.boundary.width - 2f)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
}
