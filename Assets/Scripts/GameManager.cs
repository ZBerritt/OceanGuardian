using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Add any sort of save data here
    public List<TrashItemData> inventory;
    public int boatUpgradeLevel = 0;
    public int boatNetLevel = 0;
    public Vector2 playerPosition;
    public int trashDensity = 100;

    public event Action OnDayEnd;
    private AudioSource sfxSource;

    [SerializeField] private AudioClip dayEndSfx;

    public TrashDatabase TrashDatabase;
    public BoatDatabase BoatDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inventory = new List<TrashItemData>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CollectTrash(TrashItemData item)
    {
        if (InventoryCapacity() == InventoryUsed())
            return false;
        if (!item.Big && boatNetLevel < 1)
            return false;
        inventory.Add(item);
        Debug.Log("Collected: " + item.Name);
        return true;
    }

    public int InventoryCapacity()
    {
        return BoatDatabase.GetBoatType(boatUpgradeLevel, boatNetLevel).InventoryCapacity;
    }

    public int InventoryUsed()
    {
        return inventory.Count;
    }

    public List<TrashItemData> getInventory()
    {
        return inventory;
    }

    public void EndDay()
    {
        OnDayEnd?.Invoke();

        if (dayEndSfx != null)
        {
            sfxSource.PlayOneShot(dayEndSfx);
        }

        StartCoroutine(LoadTrashFacility());
    }

    private IEnumerator LoadTrashFacility()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("TrashFacilityScene");
    }

}
