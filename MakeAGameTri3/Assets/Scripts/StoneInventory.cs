using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton that tracks the player's stone count.
/// Other systems read and modify stone through this class.
/// </summary>
public class StoneInventory : MonoBehaviour
{
    public static StoneInventory Instance { get; private set; }

    /// <summary>Fires whenever the stone count changes. Passes the new total.</summary>
    public UnityEvent<int> OnStoneChanged;

    private int _stone;

    /// <summary>Current stone count.</summary>
    public int Stone => _stone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Adds the given amount of stone and fires OnStoneChanged.</summary>
    public void AddStone(int amount)
    {
        if (amount <= 0) return;

        _stone += amount;
        OnStoneChanged.Invoke(_stone);
        Debug.Log($"StoneInventory: +{amount} stone. Total: {_stone}");
    }

    /// <summary>
    /// Attempts to spend the given amount of stone.
    /// Returns true and deducts if the player has enough; returns false otherwise.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (amount <= 0 || _stone < amount) return false;

        _stone -= amount;
        OnStoneChanged.Invoke(_stone);
        return true;
    }

    /// <summary>Returns true if the player can afford the given cost.</summary>
    public bool CanAfford(int amount) => _stone >= amount;
}
