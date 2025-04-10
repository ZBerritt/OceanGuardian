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
    private List<TrashItemData> inventory;
    
    [Header("General")]
    [Range(0, 2)] // Change if we add more upgrades
    public int boatUpgradeLevel = 0;
    [Range(0, 1)] // Change if we add more upgrades
    public int boatNetLevel = 0;

    [Header("Facility")]
    public Vector2 playerPosition;

    [Header("Ocean")]
    [Range(0, 100)]
    public int trashDensity = 100;
    public event Action OnDayEnd;
    [SerializeField] private AudioClip dayEndSfx;

    [Header("Internals")]
    public TrashDatabase TrashDatabase;
    public BoatDatabase BoatDatabase;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        inventory = new List<TrashItemData>();
        sfxSource = gameObject.AddComponent<AudioSource>();
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
