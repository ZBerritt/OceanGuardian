using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TrashItem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string tagToDetect = "Player";
    [SerializeField] private float collectDelay = 0.2f;
    [SerializeField] private GameObject collectEffect;

    [Header("Rendering")]

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 0.7f;

    private TrashItemData itemData;
    private GameManager gameManager;
    private SpriteRenderer itemRenderer;
    private bool isCollected = false;

    private void Awake()
    {
        itemRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        isCollected = false;
        itemData = gameManager.TrashDatabase.GetRandomItem();
        itemRenderer.sprite = itemData.ItemSprite;
    }

    private void OnEnable()
    {
        //isCollected = false;
        //itemData = gameManager.TrashDatabase.GetRandomItem();
        //itemRenderer.sprite = itemData.ItemSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToDetect))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected)
            return;

        bool success = GameManager.Instance.CollectTrash(itemData);

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
