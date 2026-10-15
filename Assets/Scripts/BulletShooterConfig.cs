using UnityEngine;

[CreateAssetMenu(fileName = "BulletShooterConfig", menuName = "Systems/Bullet Shooter Config")]
public class BulletShooterConfig : ScriptableObject
{
    private const int DefaultSafetyBufferMultiplier = 2;
    private const float SpeedWarningThreshold = 200f;

    [Header("Projectile Settings")]
    [SerializeField, Tooltip("Префаб снаряда с компонентом Bullet")]
    private Bullet _bulletPrefab;

    [SerializeField, Min(0.1f)] private float _bulletSpeed = 10f;

    [SerializeField, Tooltip("Время жизни снаряда до возврата в пул (секунды)")]
    [Min(0.5f)] private float _bulletLifetime = 5f;

    [Header("Firing Settings")]
    [SerializeField, Min(0.1f)] private float _shootInterval = 1f;

    [SerializeField, Tooltip("Смещение точки спавна снаряда вперед от центра стрелка")]
    [Min(0f)] private float _spawnOffsetDistance = 1f;

    [Header("Pool Settings")]
    [SerializeField, Min(1)] private int _initialPoolCapacity = 15;

    [SerializeField, Tooltip("Жесткий лимит памяти пула снарядов")]
    [Min(1)] private int _maxPoolCapacity = 30;

    public Bullet BulletPrefab => _bulletPrefab;
    public float BulletSpeed => _bulletSpeed;
    public float BulletLifetime => _bulletLifetime;
    public float ShootInterval => _shootInterval;
    public float SpawnOffsetDistance => _spawnOffsetDistance;
    public int InitialPoolCapacity => _initialPoolCapacity;
    public int MaxPoolCapacity => _maxPoolCapacity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxPoolCapacity < _initialPoolCapacity)
        {
            _maxPoolCapacity = _initialPoolCapacity * DefaultSafetyBufferMultiplier;

            Debug.LogWarning(
                $"[{nameof(BulletShooterConfig)}] {name}: MaxPoolCapacity не может быть меньше InitialPoolCapacity. Установлен безопасный буфер (x{DefaultSafetyBufferMultiplier}).",
                this);
        }

        if (_bulletSpeed > SpeedWarningThreshold)
        {
            Debug.LogWarning($"[{nameof(BulletShooterConfig)}] {name}: Скорость снаряда превышает рекомендованный лимит ({SpeedWarningThreshold})!", this);
        }

        if (_bulletPrefab == null)
        {
            Debug.LogWarning($"[{nameof(BulletShooterConfig)}] {name}: Не забудьте назначить префаб снаряда!", this);
        }
    }
#endif
}