using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Add any sort of save data here
    private List<TrashItemData> inventory;
    public int boatUpgradeLevel = 0;
    public int boatNetLevel = 0;
    public Vector2 playerPosition;
    public int trashDensity = 100;

    public TrashDatabase TrashDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        inventory = new List<TrashItemData>();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(this);
    }

    public void LoadGame()
    {

    }

    public bool CollectTrash(TrashItemData item)
    {
        if (InventoryCapacity() == InventoryUsed())
            return false;
        inventory.Add(item);
        Debug.Log("Collected: " + item.Name);
        return true;
    }

    public int InventoryCapacity()
    {
        switch (boatUpgradeLevel)
        {
            case 0:
            default:
                return 25;
            case 1:
                return 50;
            case 2: 
                return 100;
        }
    }

    public int InventoryUsed()
    {
        return inventory.Count;
    }

}
