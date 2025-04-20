using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TrashGenerator : MonoBehaviour
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

    [Header("Water")]
    [SerializeField] private Color cleanColor;
    [SerializeField] private Color dirtyColor;
    [SerializeField] private TilemapRenderer waterRenderer;
    [SerializeField] private TilemapRenderer borderRenderer;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
    }

    private void Start()
    {
        GenerateColor();
        GenerateObstacles();
        GenerateTrash(GameManager.Instance.trashDensity);
    }

    private void GenerateColor()
    {
        Debug.Log(cleanColor);
        Debug.Log(dirtyColor);
        int density = GameManager.Instance.trashDensity;
        float t = density / 100f;
        Color resultColor = Color.Lerp(cleanColor, dirtyColor, t);

        waterRenderer.material.color = resultColor;
        borderRenderer.material.color = resultColor;
    }

    public void GenerateObstacles()
    {
        BoundsInt bounds = tilemap.cellBounds;

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
            Vector3 worldPosition = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
            worldPosition.z -= 2;
            GameObject obstacle = Instantiate(obstaclePrefab, worldPosition, Quaternion.identity);
            obstacle.GetComponent<SpriteRenderer>().sprite = sprite;   
            obstacle.name = $"Obstacle_{x}_{y}";
        }
    }

    public void GenerateTrash(int trashDensityPercent)
    {
        // Ensure density is within valid range
        float density = Mathf.Clamp(trashDensityPercent, 0, 100) / 100f;

        BoundsInt bounds = tilemap.cellBounds;

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
            Vector3 worldPosition = tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));

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
