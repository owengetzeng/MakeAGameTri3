using UnityEngine;

/// <summary>
/// Moves in a set direction, deals damage on contact with an enemy, then destroys itself.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifetime = 4f;

    private Rigidbody2D _rigidbody;
    private Vector2 _direction;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        _rigidbody.linearVelocity = _direction * moveSpeed;
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Sets the travel direction immediately after instantiation.
    /// </summary>
    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyController>() != null)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
