using System.Collections;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    private const float MinDirectionDistanceSquared = 0.001f;

    [Header("Dependencies")]
    [SerializeField] private BulletShooterConfig _config;
    [SerializeField] private BulletPool _bulletPool;
    [SerializeField] private WaypointMover _targetEntity;

    private ITarget _target;
    private WaitForSeconds _shootDelay;
    private Coroutine _shootRoutine;
    private bool _isInitialized;
    private bool _isFaulted;

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        if (!ValidateDependencies())
        {
            _isFaulted = true;
            return;
        }

        _bulletPool.Initialize();
        _target = _targetEntity;
        _shootDelay = new WaitForSeconds(_config.ShootInterval);
        _isInitialized = true;

        StartShooting();
    }

    private bool ValidateDependencies()
    {
        if (_config == null || _bulletPool == null || _targetEntity == null)
        {
            Debug.LogError($"[{nameof(BulletShooter)}] {name}: Зависимости не назначены! Остановка.", this);
            return false;
        }

        return true;
    }

    private void StartShooting()
    {
        StopShooting();
        _shootRoutine = StartCoroutine(ShootRoutine());
    }

    private void StopShooting()
    {
        if (_shootRoutine != null)
        {
            StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (isActiveAndEnabled && !_isFaulted)
        {
            Shoot();
            yield return _shootDelay;
        }
    }

    private void Shoot()
    {
        if (_target == null)
        {
            Debug.LogWarning($"[{nameof(BulletShooter)}] {name}: Цель отсутствует! Остановка стрельбы.", this);
            _isFaulted = true;
            StopShooting();
            return;
        }

        if (!_bulletPool.TryGetBullet(out Bullet bullet))
        {
            return;
        }

        Vector3 direction = GetShootDirection();
        Vector3 spawnPosition = transform.position + (direction * _config.SpawnOffsetDistance);
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, direction);

        bullet.Initialize(spawnPosition, spawnRotation, _config.BulletLifetime);
        bullet.gameObject.SetActive(true);
        bullet.Launch(direction * _config.BulletSpeed);
    }

    private Vector3 GetShootDirection()
    {
        Vector3 offset = _target.Position - transform.position;

        if (Vector3.SqrMagnitude(offset) <= MinDirectionDistanceSquared)
        {
            return transform.forward;
        }

        return offset.normalized;
    }

    private void OnDestroy()
    {
        StopShooting();
        _isInitialized = false;
    }
}