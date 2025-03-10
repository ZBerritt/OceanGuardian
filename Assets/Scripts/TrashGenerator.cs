using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrashGenerator : MonoBehaviour
{
    [Header("Configuration")]
    [Range(0.01f, 0.25f)]
    [SerializeField] private float maxTrashPercent = 0.25f;
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private Transform trashContainer;

    private void Awake()
    {
        ValidateComponents();
    }

    private void Start()
    {
        if (!ValidateComponents())
            return;

        int trashDensity = 50; // Default value

        // Get trash density from GameManager if available
        if (GameManager.Instance != null)
        {
            trashDensity = GameManager.Instance.trashDensity;
        }

        GenerateTrash(trashDensity);
    }

    private bool ValidateComponents()
    {
        bool isValid = true;

        if (targetTilemap == null)
        {
            Debug.LogError($"[{gameObject.name}] TrashGenerator: Target Tilemap is not assigned!");
            isValid = false;
        }

        if (trashPrefab == null)
        {
            Debug.LogError($"[{gameObject.name}] TrashGenerator: Trash Prefab is not assigned!");
            isValid = false;
        }

        // Create trash container if not assigned
        if (trashContainer == null)
        {
            trashContainer = new GameObject("TrashContainer").transform;
            trashContainer.SetParent(transform);
        }

        return isValid;
    }

    public void GenerateTrash(int trashDensityPercent)
    {
        // Early return if components are invalid
        if (!ValidateComponents())
            return;

        // Ensure density is within valid range
        float density = Mathf.Clamp(trashDensityPercent, 0, 100) / 100f;

        // Refresh the tilemap bounds
        targetTilemap.CompressBounds();
        BoundsInt bounds = targetTilemap.cellBounds;

        // Calculate how many trash items to create
        int width = bounds.size.x;
        int height = bounds.size.y;
        int tileCount = width * height;
        int maxTrashCount = Mathf.RoundToInt(tileCount * maxTrashPercent * density);

        Debug.Log($"Generating trash: Tilemap size {width}x{height}, Total tiles: {tileCount}, Target trash count: {maxTrashCount}");

        // Clear any existing trash
        ClearExistingTrash();

        // Find valid positions for trash
        HashSet<Vector3Int> trashPositions = new HashSet<Vector3Int>();
        int attempts = 0;
        int maxAttempts = maxTrashCount * 3; // Avoid infinite loops

        while (trashPositions.Count < maxTrashCount && attempts < maxAttempts)
        {
            attempts++;

            // Generate position within bounds
            int x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            int y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
            Vector3Int cellPosition = new Vector3Int(x, y, 0);

            // Check if this position has a tile and isn't already chosen
            if (targetTilemap.HasTile(cellPosition) && !trashPositions.Contains(cellPosition))
            {
                trashPositions.Add(cellPosition);
            }
        }

        // Instantiate trash at each position
        foreach (Vector3Int cellPos in trashPositions)
        {
            // Convert cell position to world position
            Vector3 worldPosition = targetTilemap.GetCellCenterWorld(cellPos);

            // Instantiate the trash prefab
            GameObject trash = Instantiate(trashPrefab, worldPosition + Vector3.back, Quaternion.identity, trashContainer);
            trash.name = $"Trash_{cellPos.x}_{cellPos.y}";
        }

        Debug.Log($"Trash generation complete. Created {trashPositions.Count} trash objects.");
    }

    private void ClearExistingTrash()
    {
        // Remove any existing trash items
        if (trashContainer != null)
        {
            int childCount = trashContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(trashContainer.GetChild(i).gameObject);
            }
        }
    }

    // Public method to clear and regenerate trash with a new density
    public void RegenerateTrash(int newDensity)
    {
        GenerateTrash(newDensity);
    }
}
