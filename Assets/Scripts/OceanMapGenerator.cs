using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OceanMapGenerator : MonoBehaviour
{
    [Header("Obstacles")]
    [SerializeField] private int numObstacles;
    [SerializeField] private List<Sprite> obstacleSprites;
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Trash")]
    [SerializeField] private int maxTrash;
    [Range(0f, 1f)]
    [SerializeField] private float clumpPercent;
    [SerializeField] private GameObject trashPrefab;

    [Header("Map Generator")]
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap borderTilemap;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase borderTile;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int borderThickness;

    [Header("Water Color")]
    [SerializeField] private Color cleanColor;
    [SerializeField] private Color dirtyColor;
    [SerializeField] private TilemapRenderer waterRenderer;
    [SerializeField] private TilemapRenderer borderRenderer;

    private void Start()
    {
        GenerateMap();
        GenerateColor();
        GenerateObstacles();
        GenerateTrash();
    }

    public void GenerateMap()
    {
        if (waterTilemap == null || waterTile == null || borderTilemap == null || borderTile == null)
        {
            Debug.LogError("Tilemaps or tiles not assigned!");
            return;
        }

        // Clear existing tiles
        waterTilemap.ClearAllTiles();
        borderTilemap.ClearAllTiles();

        // Calculate boundaries with center at (0,0)
        int startX = -width / 2;
        int startY = -height / 2;
        int endX = startX + width;
        int endY = startY + height;

        // Generate water tiles first (for the entire area)
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
            }
        }

        // Generate border tiles separately
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                // Check if position is within the border area
                bool isInBorder =
                    x < startX + borderThickness ||
                    x >= endX - borderThickness ||
                    y < startY + borderThickness ||
                    y >= endY - borderThickness;

                if (isInBorder)
                {
                    borderTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
                }
            }
        }

        // Refresh the composite collider if needed
        CompositeCollider2D collider = borderTilemap.GetComponent<CompositeCollider2D>();
        if (collider != null)
        {
            TilemapCollider2D tilemapCollider = borderTilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
            }

            // Refresh the composite collider
            collider.generationType = CompositeCollider2D.GenerationType.Manual;
            collider.GenerateGeometry();
        }

        // Compress bounds for trash generation
        waterTilemap.CompressBounds();
    }

    private void GenerateColor()
    {
        int density = GameManager.Instance.trashDensity;
        float t = density / 100f;
        Color resultColor = Color.Lerp(cleanColor, dirtyColor, t);

        waterRenderer.material.color = resultColor;
        borderRenderer.material.color = resultColor;
    }

    public void GenerateObstacles()
    {
        BoundsInt bounds = waterTilemap.cellBounds;

        for (int i = 0; i < numObstacles; i++)
        {
            // Generate positions
            int x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            int y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);

            Sprite sprite = obstacleSprites[UnityEngine.Random.Range(0, obstacleSprites.Count - 1)];
            
            // Can't be too close to the boat
            if (Mathf.Abs(x) <= 2 || Mathf.Abs(y) <= 2)
            {
                continue;
            }

            // Place
            Vector3 worldPosition = waterTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
            worldPosition.z -= 2;
            GameObject obstacle = Instantiate(obstaclePrefab, worldPosition, Quaternion.identity);
            obstacle.GetComponent<SpriteRenderer>().sprite = sprite;   
            obstacle.name = $"Obstacle_{x}_{y}";
        }
    }

    public void GenerateTrash()
    {
        // Ensure density is within valid range
        float density = Mathf.Clamp(GameManager.Instance.trashDensity, 0, 100) / 100f;

        BoundsInt bounds = waterTilemap.cellBounds;

        // Calculate how many trash items to create
        int clumps = Mathf.RoundToInt(maxTrash * density);

        Debug.Log($"Generating trash clumps: {clumps}");

        // Find valid positions for trash
        HashSet<Vector2Int> trashPositions = new();

        for (int i = 0; i < clumps; i++)
        {
            // Position
            int x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            int y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);

            // Can't be too close to the boat
            if (Mathf.Abs(x) <= 2 || Mathf.Abs(y) <= 2)
            {
                continue;
            }

            Vector2Int cellPosition = new(x, y);

            float shouldClump = UnityEngine.Random.Range(0f, 1);
            if (shouldClump < clumpPercent)
            {
                trashPositions.UnionWith(GenerateClump(cellPosition, cellPosition, new HashSet<Vector2Int>()));
            }
            else
            {
                trashPositions.Add(cellPosition);
            }
        }

        // Instantiate trash at each position
        foreach (Vector2Int pos in trashPositions)
        {
            // Convert cell position to world position
            Vector3 worldPosition = waterTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));

            // Apply a natural random shift to each position
            worldPosition.x += UnityEngine.Random.Range(-1f, 1f);
            worldPosition.y += UnityEngine.Random.Range(-1f, 1f);
            Mathf.Clamp(worldPosition.x, 0f, bounds.xMax);
            Mathf.Clamp(worldPosition.y, 0f, bounds.yMax);
            worldPosition.z -= 1;

            // Instantiate the trash prefab
            GameObject trash = Instantiate(trashPrefab, worldPosition, Quaternion.identity);
            trash.name = $"Trash_{pos.x}_{pos.y}";
        }

        Debug.Log($"Trash generation complete. Created {trashPositions.Count} trash objects.");
    }

    // Recursive function for generating our "trash clumps"
    private HashSet<Vector2Int> GenerateClump(Vector2Int center, Vector2Int currentPosition, HashSet<Vector2Int> positions, int maxRecursionDepth = 10)
    {
        if (maxRecursionDepth <= 0 || positions.Contains(currentPosition))
        {
            return positions;
        }

        positions.Add(currentPosition);

        int distanceFromCenter = Mathf.Max(
            Mathf.Abs(center.x - currentPosition.x),
            Mathf.Abs(center.y - currentPosition.y)
        );

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        foreach (Vector2Int direction in directions)
        {
            int probabilityDivisor = 1 << distanceFromCenter; // Using 2^n
            if (UnityEngine.Random.Range(0, probabilityDivisor) == 0)
            {
                Vector2Int nextPosition = currentPosition + direction;
                positions = GenerateClump(
                    center,
                    nextPosition,
                    positions,
                    maxRecursionDepth - 1
                );
            }
        }

        return positions;
    }
}
