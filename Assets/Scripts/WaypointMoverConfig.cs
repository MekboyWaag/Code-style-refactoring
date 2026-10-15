using UnityEngine;

[CreateAssetMenu(fileName = "WaypointMoverConfig", menuName = "Systems/Waypoint Mover Config")]
public class WaypointMoverConfig : ScriptableObject
{
    private const float SpeedWarningThreshold = 100f;

    [Header("Movement Settings")]
    [SerializeField, Min(0.1f)] private float _moveSpeed = 3f;

    [SerializeField, Tooltip("Дистанция, на которой точка маршрута считается достигнутой")]
    [Min(0.05f)] private float _waypointThreshold = 0.2f;

    public float MoveSpeed => _moveSpeed;
    public float WaypointThreshold => _waypointThreshold;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_moveSpeed > SpeedWarningThreshold)
        {
            Debug.LogWarning($"[{nameof(WaypointMoverConfig)}] {name}: Скорость движения превышает рекомендованный порог ({SpeedWarningThreshold})!", this);
        }
    }
#endif
}