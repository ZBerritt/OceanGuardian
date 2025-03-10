using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TrashItem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string[] tagsToDetect = { "Player", "PlayerBoat" };
    [SerializeField] private bool destroyOnCollect = true;
    [SerializeField] private float collectDelay = 0.2f;
    [SerializeField] private GameObject collectEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 0.7f;

    [Header("Events")]
    [SerializeField] private UnityEvent onTrashCollected;
    [SerializeField] private UnityEvent<TrashItem> onTrashCollectedWithReference;

    private bool isCollected = false;

    private void Awake()
    {
        // Ensure we have a valid collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError($"TrashItem on {gameObject.name} requires a Collider2D component!");
        }
        else if (!collider.isTrigger)
        {
            // Make sure the collider is set as a trigger
            collider.isTrigger = true;
            Debug.Log($"Set collider on {gameObject.name} to trigger mode.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;

        // Check if the colliding object has one of our tags to detect
        bool shouldCollect = false;
        foreach (string tag in tagsToDetect)
        {
            if (other.CompareTag(tag))
            {
                shouldCollect = true;
                break;
            }
        }

        if (shouldCollect)
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected)
            return;

        isCollected = true;

        // Notify any listeners
        onTrashCollected?.Invoke();
        onTrashCollectedWithReference?.Invoke(this);

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

        // Track the collection
        TrackCollection();

        // Handle destruction
        if (destroyOnCollect)
        {
            // Can optionally animate before destruction
            Destroy(gameObject, collectDelay);
        }
        else
        {
            // Just disable the GameObject
            gameObject.SetActive(false);
        }
    }

    private void TrackCollection()
    {
        // Report to any score or progression systems
        if (GameManager.Instance != null)
        {
            // Call your GameManager's methods to track trash collection
            // For example:
            // GameManager.Instance.TrashCollected();

            // If you have a player stats system:
            // GameManager.Instance.AddPlayerScore(10);
        }
    }

    // Public method to allow forced collection from elsewhere
    public void ForceCollect()
    {
        Collect();
    }
}