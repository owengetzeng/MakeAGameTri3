using UnityEngine;

/// <summary>
/// Placed on a "stone square" GameObject.
/// When the player's mining hit collider enters the trigger on this GameObject,
/// the player receives stone. The solid BoxCollider2D on the same object blocks movement.
/// </summary>
public class StoneDeposit : MonoBehaviour
{
    [Header("Yield")]
    [SerializeField] private int stonePerHit = 1;

    /// <summary>
    /// Called by the physics system when another trigger enters this object's trigger collider.
    /// Only the player's mining hit (tagged "MiningHit") awards stone.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerMiner.MiningHitTag)) return;

        if (StoneInventory.Instance == null)
        {
            Debug.LogWarning("StoneDeposit: No StoneInventory found in scene.");
            return;
        }

        StoneInventory.Instance.AddStone(stonePerHit);
    }
}
