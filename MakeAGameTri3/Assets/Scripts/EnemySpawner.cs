using UnityEngine;

/// <summary>
/// Periodically spawns enemies at random positions on a circle around the Base.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 20;

    private float _spawnTimer;

    private void Update()
    {
        if (CountActiveEnemies() >= maxEnemies) return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    /// <summary>
    /// Spawns a single enemy at a random point on the spawn circle.
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: No enemy prefab assigned.");
            return;
        }

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(offset.x, offset.y, 0f);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private int CountActiveEnemies()
    {
        return FindObjectsByType<EnemyController>(FindObjectsSortMode.None).Length;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
