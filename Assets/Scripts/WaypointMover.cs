using UnityEngine;

public class WaypointMover : MonoBehaviour, ITarget
{
    private const float MinRotationDistanceSquared = 0.001f;

    [Header("Dependencies")]
    [SerializeField] private WaypointMoverConfig _config;

    [SerializeField, Tooltip("Родительский объект, содержащий дочерние точки маршрута")]
    private Transform _waypointsContainer;

    private Transform[] _waypoints;
    private int _currentWaypointIndex;
    private bool _isFaulted;

    public Vector3 Position => transform.position;

    private void Awake()
    {
        if (!TryInitializeWaypoints())
        {
            _isFaulted = true;
            return;
        }

        Transform firstWaypoint = _waypoints[_currentWaypointIndex];

        if (firstWaypoint != null)
        {
            UpdateRotation(firstWaypoint.position);
        }
    }

    private void Update()
    {
        if (_isFaulted)
        {
            return;
        }

        HandleMovement();
    }

    private bool TryInitializeWaypoints()
    {
        if (_config == null)
        {
            Debug.LogError($"[{nameof(WaypointMover)}] {name}: Конфиг движения не назначен!", this);
            return false;
        }

        if (_waypointsContainer == null)
        {
            Debug.LogError($"[{nameof(WaypointMover)}] {name}: Контейнер точек маршрута не назначен!", this);
            return false;
        }

        int waypointCount = _waypointsContainer.childCount;

        if (waypointCount == 0)
        {
            Debug.LogError($"[{nameof(WaypointMover)}] {name}: Контейнер {_waypointsContainer.name} не содержит дочерних точек!", this);
            return false;
        }

        _waypoints = new Transform[waypointCount];

        for (int i = 0; i < waypointCount; i++)
        {
            _waypoints[i] = _waypointsContainer.GetChild(i);
        }

        return true;
    }

    private bool IsWaypointReached(Vector3 targetPosition)
    {
        float thresholdSquared = _config.WaypointThreshold * _config.WaypointThreshold;
        float distanceSquared = Vector3.SqrMagnitude(transform.position - targetPosition);

        return distanceSquared <= thresholdSquared;
    }

    private void HandleMovement()
    {
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];

        if (targetWaypoint == null)
        {
            Debug.LogWarning($"[{nameof(WaypointMover)}] {name}: Точка маршрута с индексом {_currentWaypointIndex} отсутствует (Null)! Пропускаем.", this);
            SelectNextWaypoint();
            return;
        }

        Vector3 targetPosition = targetWaypoint.position;

        UpdateRotation(targetPosition);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _config.MoveSpeed * Time.deltaTime);

        if (IsWaypointReached(targetPosition))
        {
            SelectNextWaypoint();
        }
    }

    private void SelectNextWaypoint()
    {
        _currentWaypointIndex = ++_currentWaypointIndex % _waypoints.Length;
    }

    private void UpdateRotation(Vector3 targetPosition)
    {
        Vector3 offset = targetPosition - transform.position;

        if (Vector3.SqrMagnitude(offset) > MinRotationDistanceSquared)
        {
            transform.rotation = Quaternion.LookRotation(offset.normalized);
        }
    }
}