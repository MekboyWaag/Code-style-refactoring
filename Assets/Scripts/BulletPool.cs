using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private BulletShooterConfig _config;

    private ObjectPool<Bullet> _pool;
    private bool _isInitialized;
    private bool _isFaulted;

    public void Initialize()
    {
        if (_isInitialized || _isFaulted)
        {
            return;
        }

        if (!ValidateConfig())
        {
            _isFaulted = true;
            return;
        }

        _pool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: null,
            actionOnRelease: ReleaseBullet,
            actionOnDestroy: DestroyBullet,
            collectionCheck: false,
            defaultCapacity: _config.InitialPoolCapacity,
            maxSize: _config.MaxPoolCapacity
        );

        PopulatePool();
        _isInitialized = true;
    }

    public bool TryGetBullet(out Bullet bullet)
    {
        if (!_isInitialized || _isFaulted)
        {
            bullet = null;
            return false;
        }

        bullet = _pool.Get();
        return true;
    }

    private bool ValidateConfig()
    {
        if (_config == null)
        {
            Debug.LogError($"[{nameof(BulletPool)}] {name}: Конфиг не назначен!", this);
            return false;
        }

        if (_config.BulletPrefab == null)
        {
            Debug.LogError($"[{nameof(BulletPool)}] {name}: В конфиге {_config.name} не назначен префаб снаряда!", this);
            return false;
        }

        return true;
    }

    private void PopulatePool()
    {
        var warmUpCache = ListPool<Bullet>.Get();

        for (int i = 0; i < _config.InitialPoolCapacity; i++)
        {
            warmUpCache.Add(_pool.Get());
        }

        foreach (Bullet bullet in warmUpCache)
        {
            _pool.Release(bullet);
        }

        ListPool<Bullet>.Release(warmUpCache);
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(_config.BulletPrefab);

        bullet.gameObject.SetActive(false);
        bullet.LifetimeEnded += OnBulletLifetimeEnded;

        return bullet;
    }

    private void ReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnBulletLifetimeEnded(Bullet bullet)
    {
        if (!_isInitialized)
        {
            DestroyBullet(bullet);
            return;
        }

        _pool.Release(bullet);
    }

    private void DestroyBullet(Bullet bullet)
    {
        if (bullet != null)
        {
            bullet.LifetimeEnded -= OnBulletLifetimeEnded;
            Destroy(bullet.gameObject);
        }
    }

    private void OnDestroy()
    {
        _isInitialized = false;
        _pool?.Dispose();
    }
}