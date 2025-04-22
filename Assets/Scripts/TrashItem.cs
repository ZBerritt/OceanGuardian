using NUnit.Framework.Interfaces;
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

    private void Start()
    {
        itemRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        isCollected = false;
        data = gameManager.TrashDatabase.GetRandomItem();
        itemRenderer.sprite = data.ItemSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected)
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

    public void Init(TrashItemData newData)
    {
        data = newData;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr.sprite = data.ItemSprite;
        }
        sr.sprite = data.ItemSprite;
    }
}
