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

    void Awake()
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        _isActive = false;
        _player.AddBalls(1);
        StartCoroutine(DeactivateBall());
    }

    private IEnumerator DeactivateBall()
    {
        _isActive = false;
        
        yield return new WaitForSeconds(_deactivateDelay);
        
        _player.ReturnBallToPool(gameObject);
    }
}

