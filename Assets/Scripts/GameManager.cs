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
    [UnityEngine.Range(0, 1)]
    public int boatNetLevel = 0;

    [Header("Player Parameters")]
    public Vector2 playerPosition;
    public TimeOfDay timeOfDay = TimeOfDay.Morning;
    public int day = 0;

    [Header("Trash Parameters")]
    public List<TrashItemData> inventory;
    [UnityEngine.Range(0, 100)]
    public int trashDensity = 100;

    [Header("SFX")]
    [SerializeField] private AudioClip dayEndSfx;
    private AudioSource sfxSource;

    [Header("Buffer Images")]
    [SerializeField] private Sprite morningBuffer;
    [SerializeField] private Sprite eveningBuffer;

    [Header("Scriptable Databases")]
    public TrashDatabase TrashDatabase;
    public BoatDatabase BoatDatabase;

    public List<int> unlockedTrashCans; // Defaults

    public int doubloons = 0;
    public event Action OnDayEnd;

    private int trawlerCost = 500;
    private int skimmerCost = 1000;
    private int netUpgradeCost = 100;
    private int binUpgradeCost = 50;

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

    // Ends ocean
    public void EndDay()
    {
        OnDayEnd?.Invoke();

        if (dayEndSfx != null)
        {
            sfxSource.PlayOneShot(dayEndSfx);
        }

        timeOfDay = TimeOfDay.Evening;
        StartCoroutine(WaitAndLoadFacility());
    }

    private IEnumerator WaitAndLoadFacility()
    {
        yield return new WaitForSeconds(2f);
        LoadTrashFacility();
    }

    public void LoadOcean()
    {
        StartCoroutine(BufferTransitionController.Instance.TransitionScene("OceanScene", 5f, morningBuffer));
    }

    public void LoadTrashFacility()
    {
        StartCoroutine(BufferTransitionController.Instance.TransitionScene("TrashFacilityScene", 5f, eveningBuffer));
    }

    public void openWorkshopMenu()
    {
        Canvas workshopCanvas = GameObject.Find("WorkshopMenuCanvas").GetComponent<Canvas>();
        workshopCanvas.enabled = !workshopCanvas.enabled;
    }

    public void upgradeToTrawler()
    {
        Debug.Log("trawler button pressesd");
        if (Instance.doubloons >= trawlerCost && Instance.boatUpgradeLevel == 0)
        {
            Instance.doubloons -= trawlerCost;
            Instance.boatUpgradeLevel += 1;
            Debug.Log("upgraded to trawler!");
        }
    }

    public void upgradeToSkimmer()
    {
        Debug.Log("skimmer button pressesd");
        if (Instance.doubloons >= skimmerCost && Instance.boatUpgradeLevel == 1)
        {
            Instance.doubloons -= skimmerCost;
            Instance.boatUpgradeLevel += 1;
        }
    }

    public void upgradeNet() 
    {
        Debug.Log("net button pressesd"); 
        if (Instance.doubloons >= netUpgradeCost && Instance.boatNetLevel == 0)
        {
            Instance.doubloons -= netUpgradeCost;
            Instance.boatNetLevel = 1;
        }
    }
}

public enum TimeOfDay
{
    Morning,
    Evening
}
