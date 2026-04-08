using UnityEngine;

/// <summary>
/// Moves the enemy toward the Base each physics tick.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D _rigidbody;
    private Transform _target;

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
    }
}
