using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic health component that can be attached to any entity.
/// Raises events on damage and death so other systems can react without tight coupling.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    /// <summary>Fires whenever damage is taken. Passes current health after the hit.</summary>
    public UnityEvent<float> OnDamageTaken;

    /// <summary>Fires once when health reaches zero.</summary>
    public UnityEvent OnDeath;

    private float _currentHealth;
    private bool _isDead;

    /// <summary>Current health value.</summary>
    public float CurrentHealth => _currentHealth;

    /// <summary>Maximum health value.</summary>
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    /// <summary>
    /// Reduces health by the given amount. Triggers OnDamageTaken and OnDeath as appropriate.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (_isDead || amount <= 0f) return;

        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        OnDamageTaken.Invoke(_currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Restores health by the given amount, capped at maxHealth.
    /// </summary>
    public void Heal(float amount)
    {
        if (_isDead || amount <= 0f) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        _isDead = true;
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}
