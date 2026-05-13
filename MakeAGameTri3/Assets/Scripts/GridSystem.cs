using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a checkerboard grid for tower placement.
/// Renders the grid visually, tracks occupied cells, and provides snapping utilities.
/// Place this on its own GameObject in the scene.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Grid")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int gridWidth  = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private Vector2 gridOrigin = new Vector2(-10f, -10f);

    [Header("Colors")]
    [SerializeField] private Color colorLight        = new Color(0.55f, 0.75f, 0.55f, 1f);
    [SerializeField] private Color colorDark         = new Color(0.35f, 0.55f, 0.35f, 1f);
    [SerializeField] private Color hoverValidColor   = new Color(1f,    1f,    0.4f,  0.6f);
    [SerializeField] private Color hoverInvalidColor = new Color(1f,    0.3f,  0.3f,  0.6f);

    private SpriteRenderer _gridRenderer;
    private GameObject     _hoverHighlight;
    private SpriteRenderer _hoverRenderer;

    private readonly HashSet<Vector2Int> _occupiedCells = new();

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _gridRenderer = GetComponent<SpriteRenderer>();

        BuildCheckerboardSprite();
        CreateHoverHighlight();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>Returns the world-space centre of the cell that worldPos falls in.</summary>
    public Vector2 SnapToGrid(Vector2 worldPos)
    {
        return CellCenter(WorldToCell(worldPos));
    }

    /// <summary>Converts a world position to an integer grid cell coordinate.</summary>
    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        int cx = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int cy = Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize);
        return new Vector2Int(
            Mathf.Clamp(cx, 0, gridWidth  - 1),
            Mathf.Clamp(cy, 0, gridHeight - 1)
        );
    }

    /// <summary>Returns true if worldPos is within the grid boundaries.</summary>
    public bool IsInsideGrid(Vector2 worldPos)
    {
        return worldPos.x >= gridOrigin.x
            && worldPos.x <  gridOrigin.x + gridWidth  * cellSize
            && worldPos.y >= gridOrigin.y
            && worldPos.y <  gridOrigin.y + gridHeight * cellSize;
    }

    /// <summary>Returns true if the given cell already has a tower placed in it.</summary>
    public bool IsOccupied(Vector2Int cell) => _occupiedCells.Contains(cell);

    /// <summary>Marks a cell as occupied. Call this immediately after placing a tower.</summary>
    public void OccupyCell(Vector2Int cell) => _occupiedCells.Add(cell);

    /// <summary>
    /// Moves the hover highlight to the given cell and tints it green (valid) or red (invalid).
    /// </summary>
    public void UpdateHoverHighlight(Vector2Int cell, bool valid)
    {
        if (_hoverHighlight == null) return;

        _hoverHighlight.transform.position = (Vector3)CellCenter(cell);
        _hoverRenderer.color = valid ? hoverValidColor : hoverInvalidColor;
        _hoverHighlight.SetActive(true);
    }

    /// <summary>Hides the hover highlight when the cursor leaves the grid.</summary>
    public void HideHoverHighlight()
    {
        _hoverHighlight?.SetActive(false);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private Vector2 CellCenter(Vector2Int cell)
    {
        return new Vector2(
            gridOrigin.x + cell.x * cellSize + cellSize * 0.5f,
            gridOrigin.y + cell.y * cellSize + cellSize * 0.5f
        );
    }

    /// <summary>
    /// Generates a pixel-per-cell Texture2D with alternating colors and assigns it
    /// to the SpriteRenderer so the grid fills its exact world-space bounds.
    /// </summary>
    private void BuildCheckerboardSprite()
    {
        var tex = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode   = TextureWrapMode.Clamp
        };

        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
                tex.SetPixel(x, y, ((x + y) % 2 == 0) ? colorLight : colorDark);

        tex.Apply();

        // pixelsPerUnit = 1 / cellSize so that each pixel occupies exactly cellSize world units.
        float ppu    = 1f / cellSize;
        var   sprite = Sprite.Create(tex, new Rect(0, 0, gridWidth, gridHeight), new Vector2(0.5f, 0.5f), ppu);

        _gridRenderer.sprite       = sprite;
        _gridRenderer.sortingOrder = -10; // Render behind all sprites

        // Position the sprite so its centre aligns with the grid's world centre.
        transform.position = new Vector3(
            gridOrigin.x + gridWidth  * cellSize * 0.5f,
            gridOrigin.y + gridHeight * cellSize * 0.5f,
            0f
        );
    }

    /// <summary>
    /// Creates a child GameObject used as the per-cell hover highlight.
    /// Uses a 1×1 white sprite scaled to cellSize so it fills exactly one cell.
    /// </summary>
    private void CreateHoverHighlight()
    {
        _hoverHighlight = new GameObject("HoverHighlight");
        _hoverHighlight.transform.SetParent(transform, worldPositionStays: true);

        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        _hoverRenderer = _hoverHighlight.AddComponent<SpriteRenderer>();
        _hoverRenderer.sprite       = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        _hoverRenderer.color        = hoverValidColor;
        _hoverRenderer.sortingOrder = -5; // Above grid, below towers and ghost

        _hoverHighlight.transform.localScale = Vector3.one * cellSize;
        _hoverHighlight.SetActive(false);
    }
}
