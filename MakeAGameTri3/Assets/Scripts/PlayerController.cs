using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Top-down 2D player controller. Reads movement input from the Input System
/// and drives the Rigidbody2D for physics-based movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;
    }

    /// <summary>
    /// Called by PlayerInput when the Move action fires.
    /// </summary>
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _moveInput * moveSpeed;
    }
}
