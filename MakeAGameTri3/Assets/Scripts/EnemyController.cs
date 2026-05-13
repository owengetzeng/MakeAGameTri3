using UnityEngine;

/// <summary>
/// Moves the enemy toward the Base each physics tick.
/// Deals contact damage to the player and towers while overlapping.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(HealthComponent))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Contact Damage")]
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float damageCooldown = 1f;

    private Rigidbody2D _rigidbody;
    private Transform _target;
    private float _damageTimer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;

        GameObject baseObject = GameObject.FindWithTag("Base");
        if (baseObject != null)
        {
            _target = baseObject.transform;
        }
        else
        {
            Debug.LogWarning("EnemyController: No GameObject with tag 'Base' found in the scene.");
        }
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        Vector2 direction = ((Vector2)_target.position - _rigidbody.position).normalized;
        _rigidbody.linearVelocity = direction * moveSpeed;

        _damageTimer -= Time.fixedDeltaTime;
    }

    // Fires continuously while the enemy is physically in contact with another solid collider.
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDealDamage(collision.gameObject);
    }

    /// <summary>
    /// Deals contact damage to any damageable entity (player or tower), rate-limited by damageCooldown.
    /// </summary>
    private void TryDealDamage(GameObject target)
    {
        if (_damageTimer > 0f) return;

        HealthComponent health = target.GetComponent<HealthComponent>();
        if (health != null && (target.GetComponent<PlayerController>() != null
                               || target.GetComponent<TowerController>() != null))
        {
            health.TakeDamage(contactDamage);
            _damageTimer = damageCooldown;
        }
    }
}
