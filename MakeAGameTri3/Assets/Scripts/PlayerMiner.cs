using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached to the Player. When Space is pressed, spawns a short-lived trigger
/// collider just in front of (or around) the player to detect StoneDeposit objects.
/// </summary>
public class PlayerMiner : MonoBehaviour
{
    /// <summary>Tag applied to the temporary mining hit collider.</summary>
    public const string MiningHitTag = "MiningHit";

    [Header("Mining")]
    [SerializeField] private float hitRadius  = 0.8f;
    [SerializeField] private float hitDuration = 0.05f;

    private bool _wasSpaceHeld;

    private void Update()
    {
        bool isSpaceHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        if (isSpaceHeld && !_wasSpaceHeld)
            StartCoroutine(SpawnHitCollider());

        _wasSpaceHeld = isSpaceHeld;
    }

    /// <summary>
    /// Creates a child trigger collider for one frame-width of time so that any
    /// overlapping StoneDeposit receives an OnTriggerEnter2D callback.
    /// </summary>
    private IEnumerator SpawnHitCollider()
    {
        var hitObject = new GameObject("MiningHit");
        hitObject.tag = MiningHitTag;
        hitObject.transform.SetParent(transform, worldPositionStays: false);
        hitObject.transform.localPosition = Vector3.zero;

        var rb = hitObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        var col = hitObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = hitRadius;

        yield return new WaitForSeconds(hitDuration);

        if (hitObject != null)
            Destroy(hitObject);
    }
}
