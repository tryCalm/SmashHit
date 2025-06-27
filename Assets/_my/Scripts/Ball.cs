using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _deactivateDelay = 0.3f;

    private Rigidbody _rb;
    private Collider _collider;
    private Player _player;

    private bool _isActive;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Player player)
    {
        _player = player;
    }

    public void Launch(Vector3 force)
    {
        _isActive = true;
        
        _rb.velocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.Impulse);
        StartCoroutine(DeactivateBall(5));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isActive) return;
        if (collision.gameObject.TryGetComponent(out DestructibleObject destructible))
        {
            _isActive = false;
            _player.AddBalls(3);
        }
        StartCoroutine(DeactivateBall());
    }

    private IEnumerator DeactivateBall()
    {
        _isActive = false;
        
        yield return new WaitForSeconds(_deactivateDelay);
        
        _player.ReturnBallToPool(gameObject);
    }
    
    private IEnumerator DeactivateBall(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        _player.ReturnBallToPool(gameObject);
    }
}

