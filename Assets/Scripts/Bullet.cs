using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _remainingLifetime;
    private bool _isReleased;

    public event Action<Bullet> LifetimeEnded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isReleased)
        {
            return;
        }

        UpdateLifetime();
    }

    public void Initialize(Vector3 spawnPosition, Quaternion spawnRotation, float maxLifetime)
    {
        _remainingLifetime = maxLifetime;
        _isReleased = false;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
    }

    public void Launch(Vector3 velocity)
    {
        _rigidbody.velocity = velocity;
    }

    private void UpdateLifetime()
    {
        _remainingLifetime -= Time.deltaTime;

        if (_remainingLifetime <= 0f)
        {
            Release();
        }
    }

    private void ResetPhysics()
    {
        if (_rigidbody == null || _rigidbody.isKinematic)
        {
            return;
        }

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;
        LifetimeEnded?.Invoke(this);
    }

    private void OnDisable()
    {
        ResetPhysics();
    }

    private void OnDestroy()
    {
        LifetimeEnded = null;
    }
}