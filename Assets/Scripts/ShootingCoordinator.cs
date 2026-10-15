using UnityEngine;

public class ShootingCoordinator : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField, Tooltip("Массив всех стреляющих турелей на сцене")]
    private BulletShooter[] _shooters;

    private void Start()
    {
        if (_shooters == null || _shooters.Length == 0)
        {
            Debug.LogWarning($"[{nameof(ShootingCoordinator)}] {name}: Стреляющие установки не назначены.", this);
            return;
        }

        for (int i = 0; i < _shooters.Length; i++)
        {
            BulletShooter shooter = _shooters[i];

            if (shooter == null)
            {
                Debug.LogWarning($"[{nameof(ShootingCoordinator)}] {name}: Слот стрелка с индексом {i} пуст (Null)! Пропускаем.", this);
                continue;
            }

            shooter.Initialize();
        }
    }
}