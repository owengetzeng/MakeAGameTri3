using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles grid-snapped, mouse-driven tower placement.
/// The ghost tower follows the hovered cell; a left-click places the real tower.
/// Requires a GridSystem in the scene.
/// Attach this to the same GameObject as PlayerController.
/// </summary>
public class TowerPlacer : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int        stoneCost = 3;

    [Header("Ghost")]
    [SerializeField] [Range(0f, 1f)] private float ghostAlpha = 0.4f;

    private Camera         _mainCamera;
    private GameObject     _ghostInstance;
    private SpriteRenderer[] _ghostRenderers;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        CreateGhost();
    }

    private void Update()
    {
        Vector2    mouseWorld = GetMouseWorldPosition();
        GridSystem grid       = GridSystem.Instance;

        // Hide everything when the cursor is outside the grid.
        if (grid == null || !grid.IsInsideGrid(mouseWorld))
        {
            SetGhostVisible(false);
            grid?.HideHoverHighlight();
            return;
        }

        Vector2Int cell    = grid.WorldToCell(mouseWorld);
        Vector2    snapped = grid.SnapToGrid(mouseWorld);
        bool       canAfford = StoneInventory.Instance == null || StoneInventory.Instance.CanAfford(stoneCost);
        bool       valid   = !grid.IsOccupied(cell) && canAfford;

        SetGhostVisible(true);
        UpdateGhost(snapped, valid);
        grid.UpdateHoverHighlight(cell, valid);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryPlaceTower(cell, snapped);
    }

    private void OnDestroy()
    {
        if (_ghostInstance != null)
            Destroy(_ghostInstance);
    }

    // -------------------------------------------------------------------------
    // Ghost
    // -------------------------------------------------------------------------

    /// <summary>
    /// Instantiates a non-functional copy of the tower prefab for placement preview.
    /// Gameplay components are disabled so it neither shoots nor collides.
    /// </summary>
    private void CreateGhost()
    {
        if (towerPrefab == null) return;

        _ghostInstance      = Instantiate(towerPrefab);
        _ghostInstance.name = "TowerGhost";

        DisableComponentIfPresent<TowerController>(_ghostInstance);
        DisableComponentIfPresent<Collider2D>(_ghostInstance);

        Rigidbody2D rb = _ghostInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        _ghostRenderers = _ghostInstance.GetComponentsInChildren<SpriteRenderer>();

        // Render the ghost on top of placed towers and the hover highlight.
        foreach (SpriteRenderer sr in _ghostRenderers)
            sr.sortingOrder = 1;

        SetGhostVisible(false);
    }

    private void UpdateGhost(Vector2 snappedPosition, bool valid)
    {
        if (_ghostInstance == null) return;

        _ghostInstance.transform.position = new Vector3(snappedPosition.x, snappedPosition.y, 0f);
        TintGhost(valid);
    }

    private void SetGhostVisible(bool visible)
    {
        if (_ghostInstance != null)
            _ghostInstance.SetActive(visible);
    }

    private void TintGhost(bool valid)
    {
        if (_ghostRenderers == null) return;

        foreach (SpriteRenderer sr in _ghostRenderers)
        {
            Color c = sr.color;
            c.a = ghostAlpha;
            c.r = 1f;
            c.g = valid ? 1f : 0.3f;
            c.b = valid ? 1f : 0.3f;
            sr.color = c;
        }
    }

    // -------------------------------------------------------------------------
    // Placement
    // -------------------------------------------------------------------------

    /// <summary>
    /// Places a tower at the centre of the given grid cell if the cell is free
    /// and the player can afford the stone cost.
    /// </summary>
    private void TryPlaceTower(Vector2Int cell, Vector2 snappedPosition)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning("TowerPlacer: No tower prefab assigned.");
            return;
        }

        GridSystem grid = GridSystem.Instance;
        if (grid == null) return;

        if (grid.IsOccupied(cell)) return;

        if (StoneInventory.Instance != null && !StoneInventory.Instance.TrySpend(stoneCost))
        {
            Debug.Log($"TowerPlacer: Not enough stone. Need {stoneCost}, have {StoneInventory.Instance.Stone}.");
            return;
        }

        Instantiate(towerPrefab, new Vector3(snappedPosition.x, snappedPosition.y, 0f), Quaternion.identity);
        grid.OccupyCell(cell);
    }

    // -------------------------------------------------------------------------
    // Utilities
    // -------------------------------------------------------------------------

    private Vector2 GetMouseWorldPosition()
    {
        if (_mainCamera == null || Mouse.current == null) return Vector2.zero;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(_mainCamera.transform.position.z);
        return _mainCamera.ScreenToWorldPoint(screenPos);
    }

    private static void DisableComponentIfPresent<T>(GameObject go) where T : Behaviour
    {
        T component = go.GetComponent<T>();
        if (component != null)
            component.enabled = false;
    }
}
