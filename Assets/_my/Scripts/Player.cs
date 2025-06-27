using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _baseSpeed = 10f;
    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _acceleration = 0.1f;
    private float _currentSpeed;
    
    [Header("Shooting")]
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private float _shootForce = 30f;
    [SerializeField] private float _shootCooldown = 0.2f;
    private bool _canShoot = true;
    private Camera _mainCamera;
    
    [Header("Ball System")]
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private int _maxBalls = 50;
    [SerializeField] private int _initialBalls = 5;
    private int _currentBalls;
    private Queue<GameObject> _ballPool = new Queue<GameObject>();
    private const int POOL_SIZE = 30;
    
    private Transform _transform;

    void Awake()
    {
        _transform = transform;
        _mainCamera = Camera.main;
        _currentSpeed = _baseSpeed;
        _currentBalls = _initialBalls;
        
        InitializeBallPool();
    }

    void Update()
    {
        HandleMovement();
        HandleShootingInput();
    }

    private void HandleMovement()
    {
        _currentSpeed = Mathf.Min(_currentSpeed + _acceleration * Time.deltaTime, _maxSpeed);
        _transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
    }

    private void HandleShootingInput()
    {
        if (_currentBalls <= 0 || !_canShoot) return;

        bool inputDetected = 
#if UNITY_EDITOR || UNITY_STANDALONE
            Input.GetMouseButtonDown(0);
#else
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
#endif

        if (inputDetected)
        {
            StartCoroutine(ShootBall());
        }
    }

    private void InitializeBallPool()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject ball = Instantiate(_ballPrefab);
            ball.SetActive(false);
            
            var ballController = ball.GetComponent<Ball>();
            ballController.Initialize(this);
            
            _ballPool.Enqueue(ball);
        }
    }

    private IEnumerator ShootBall()
    {
        _canShoot = false;
        _currentBalls--;
        
        GameObject ball = GetBallFromPool();
        ball.transform.SetPositionAndRotation(
            _shootOrigin.position, 
            _shootOrigin.rotation
        );
        
        ball.SetActive(true);

        Vector3 shootDirection = CalculateAimDirection();
        ball.GetComponent<Ball>().Launch(shootDirection * _shootForce);

        yield return new WaitForSeconds(_shootCooldown);
        _canShoot = true;
    }

    private Vector3 CalculateAimDirection()
    {
        Vector3 targetPosition = GetTargetWorldPosition();
        
        Vector3 direction = (targetPosition - _shootOrigin.position).normalized;
    
        return direction;
    }

    private Vector3 GetTargetWorldPosition()
    {
        Vector3 inputPosition = GetInputPosition();
        
        Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

        float distance = 30f;
        return ray.origin + ray.direction * distance;
    }

    private Vector3 GetInputPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#else
    return Input.GetTouch(0).position;
#endif
    }

    private GameObject GetBallFromPool()
    {
        if (_ballPool.Count > 0) 
            return _ballPool.Dequeue();
        
        GameObject newBall = Instantiate(_ballPrefab);
        newBall.GetComponent<Ball>().Initialize(this);
        return newBall;
    }

    public void ReturnBallToPool(GameObject ball)
    {
        if (!_ballPool.Contains(ball))
        {
            ball.SetActive(false);
            _ballPool.Enqueue(ball);
        }
    }

    public void AddBalls(int amount)
    {
        _currentBalls = Mathf.Min(_currentBalls + amount, _maxBalls);
    }
}

