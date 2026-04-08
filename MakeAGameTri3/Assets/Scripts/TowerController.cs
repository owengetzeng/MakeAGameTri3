using UnityEngine;

/// <summary>
/// Detects the nearest enemy within a radius and fires projectiles at it
/// at a fixed fire rate.
/// </summary>
public class TowerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;

    [Header("Firing")]
    [SerializeField] private float fireRate = 1f;  // shots per second

    private float _fireCooldown;

    private void Update()
    {
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown > 0f) return;

        Transform target = FindNearestEnemy();
        if (target == null) return;

        Fire(target);
        _fireCooldown = 1f / fireRate;
    }

    /// <summary>
    /// Returns the Transform of the nearest active enemy within detectionRadius, or null.
    /// </summary>
    private Transform FindNearestEnemy()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        Transform nearest = null;
        float nearestSqDist = detectionRadius * detectionRadius;

        foreach (EnemyController enemy in enemies)
        {
            float sqDist = ((Vector2)(enemy.transform.position - transform.position)).sqrMagnitude;
            if (sqDist <= nearestSqDist)
            {
                nearestSqDist = sqDist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Instantiates a projectile aimed at the target.
    /// </summary>
    private void Fire(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("TowerController: No projectile prefab assigned.");
            return;
        }

        Vector2 direction = ((Vector2)(target.position - transform.position)).normalized;
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
