using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Boat Parameters")]
    [UnityEngine.Range(0, 2)]
    public int boatUpgradeLevel = 0;
    [UnityEngine.Range(0, 2)]
    public int boatNetLevel = 0;

    [Header("Player Parameters")]
    public Vector2 playerPosition;

    [Header("Trash Parameters")]
    public List<TrashItemData> inventory;
    [UnityEngine.Range(0, 100)]
    public int trashDensity = 100;

    [Header("SFX")]
    [SerializeField] private AudioClip dayEndSfx;
    private AudioSource sfxSource;

    [Header("Scriptable Databases")]
    public TrashDatabase TrashDatabase;
    public BoatDatabase BoatDatabase;

    public event Action OnDayEnd;

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
