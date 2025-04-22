using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TrashItem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float collectDelay = 0.2f;
    [SerializeField] private GameObject collectEffect;
    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 0.7f;

    public TrashItemData data;
    private GameManager gameManager;
    private SpriteRenderer itemRenderer;
    private bool isCollected = false;
    private bool isInitialized = false;

    private void Awake()
    {
        itemRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        isCollected = false;
        isInitialized = true;

        UpdateVisuals();
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (itemRenderer == null)
        {
            itemRenderer = GetComponent<SpriteRenderer>();
        }

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // Get random item if none is assigned
        if (data == null && gameManager != null)
        {
            data = gameManager.TrashDatabase.GetRandomItem();
        }

        // Update sprite
        if (itemRenderer != null && data != null)
        {
            itemRenderer.sprite = data.ItemSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, TrashItemData itemData = null)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        TrashItem trashItem = instance.GetComponent<TrashItem>();

        if (itemData != null)
        {
            trashItem.data = itemData;

            SpriteRenderer sr = trashItem.GetComponent<SpriteRenderer>();
            if (sr != null && itemData.ItemSprite != null)
            {
                sr.sprite = itemData.ItemSprite;
            }
        }

        return instance;
    }

    public void Collect()
    {
        if (isCollected)
            return;

        if (GameManager.Instance == null)
            return;

        bool success = GameManager.Instance.CollectTrash(data);
        if (!success)
            return;

        isCollected = true;

        // Play collection effect if assigned
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Play collection sound if assigned
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volumeScale);
        }

        Destroy(gameObject, collectDelay);
    }
}