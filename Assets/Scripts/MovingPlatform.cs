using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public bool moveInX = true;
    public bool moveInY = false;
    public float speed = 2f;
    public float xLimit = 7f;
    public float yLimit = 5f;
    public float pauseDuration = 1f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _movingToTarget = true;
    private float _pauseTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        _targetPosition = new Vector3(
            moveInX ? _startPosition.x + xLimit : _startPosition.x,
            moveInY ? _startPosition.y + yLimit : _startPosition.y,
            _startPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (_pauseTimer > 0f)
        {
            _pauseTimer -= Time.deltaTime;
            return;
        }

        movePlatform();
    }

    private void movePlatform()
    {
        Vector3 target = _movingToTarget ? _targetPosition : _startPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _movingToTarget = !_movingToTarget;
            _pauseTimer = pauseDuration;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(
                moveInX ? startPos.x + xLimit : startPos.x,
                moveInY ? startPos.y + yLimit : startPos.y,
                startPos.z
                );
            Gizmos.color = Color.green;
            Gizmos.DrawLine( startPos, targetPos );
        }
    }
}
