using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private List<TrashData> inventory;

    // Add any sort of save data here
    public int boatUpgradeLevel = 0;
    public int boatNetLevel = 0;
    public Vector2 playerPosition;
    public int trashDensity = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        inventory = new List<TrashData>();
    }

    public bool TrashCollected(TrashData item)
    {
        if (InventoryCapacity() == InventoryUsed())
            return false;
        inventory.Add(item);
        Debug.Log("Collected" + item.Name);
        return true;
    }

    public int InventoryCapacity()
    {
        return (boatUpgradeLevel + 1) * 10;
    }

    public int InventoryUsed()
    {
        return inventory.Count;
    }

}
