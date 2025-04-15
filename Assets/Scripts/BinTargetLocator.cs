using UnityEngine;
using UnityEngine.Tilemaps;

public class BinTargetLocator : MonoBehaviour
{
    public Vector3Int tileCoordinate;
    public Vector3 offset;

    public Vector3 GetTargetPosition()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            return transform.position;
        }

        Vector3 worldPos = tilemap.CellToWorld(tileCoordinate);
        worldPos += tilemap.cellSize / 2f;
        return worldPos + offset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetTargetPosition(), 0.1f);
    }
}