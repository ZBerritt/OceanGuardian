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

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
    }

    private void Start()
    {
        GenerateObstacles();
        GenerateTrash(GameManager.Instance.trashDensity);
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
            worldPosition.z -= 1;

            // Instantiate the trash prefab
            GameObject trash = Instantiate(trashPrefab, worldPosition, Quaternion.identity);
            trash.name = $"Trash_{pos.x}_{pos.y}";
        }

        Debug.Log($"Trash generation complete. Created {trashPositions.Count} trash objects.");
    }

    private HashSet<Vector2Int> GenerateClump(Vector2Int center, Vector2Int curr, HashSet<Vector2Int> list)
    {
        // Add current to the list
        if (list.Contains(curr)) return list;
        list.Add(curr);

        int distance = Mathf.Max(Mathf.Abs(center.x - curr.x), Mathf.Abs(center.y - curr.y));

        // Right
        int shouldGenerate = UnityEngine.Random.Range(0, distance + 1);
        if (shouldGenerate == 0)
        {
            Vector2Int right = curr + Vector2Int.right;
            list = GenerateClump(center, right, list);
        }

        // Left
        shouldGenerate = UnityEngine.Random.Range(0, distance);
        if (shouldGenerate == 0)
        {
            Vector2Int left = curr + Vector2Int.left;
            list = GenerateClump(center, left, list);
        }

        // Up
        shouldGenerate = UnityEngine.Random.Range(0, distance);
        if (shouldGenerate == 0)
        {
            Vector2Int up = curr + Vector2Int.up;
            list = GenerateClump(center, up, list);
        }

        // Down
        shouldGenerate = UnityEngine.Random.Range(0, distance);
        if (shouldGenerate == 0)
        {
            Vector2Int down = curr + Vector2Int.down;
            list = GenerateClump(center, down, list);
        }

        return list;
    }
}
