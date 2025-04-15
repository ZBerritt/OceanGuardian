using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    public int numberOfItemsSorted = 0;
    public double correctPercentage = 0;
    public int itemsBackToOcean = 0;
    public int moneyEarned = 0;
    public double oceanCleanPercentage = 0;

    public BinTargetLocator recycleLocator;
    public BinTargetLocator compostLocator;
    public BinTargetLocator fishLocator;

    public Transform spawnPoint;
    public float spawnInterval = 1.5f;
    public float moveSpeed = 2f;

    private Queue<GameObject> trashQueue = new Queue<GameObject>();
    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (GameManager.Instance.inventory.Count > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnTrash();
                spawnTimer = spawnInterval;
            }
        }

        if (trashQueue.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.R)) handleBinInput(TrashType.Recyclable);
            if (Input.GetKeyDown(KeyCode.C)) handleBinInput(TrashType.Compost);
            if (Input.GetKeyDown(KeyCode.F)) handleBinInput(TrashType.Fish);
        }
    }

    void SpawnTrash()
    {
        TrashItemData data = GameManager.Instance.inventory[0];
        GameManager.Instance.inventory.RemoveAt(0);

        GameObject obj = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity);

        obj.transform.localScale = new Vector3(4f, 4f, 4f);

        TrashItem item = obj.GetComponent<TrashItem>();
        if (item == null) item = obj.AddComponent<TrashItem>();
        item.Init(data);

        TrashMove mover = obj.AddComponent<TrashMove>();
        mover.Init(moveSpeed, this);

        trashQueue.Enqueue(obj);
    }

    void handleBinInput(TrashType selectedType)
    {
        if (trashQueue.Count == 0) return;

        GameObject frontTrash = trashQueue.Dequeue();
        TrashItem item = frontTrash.GetComponent<TrashItem>();

        Vector3 targetPosition = getTargetPosition(selectedType);

        Destroy(frontTrash.GetComponent<TrashMove>());

        StartCoroutine(moveToBinAndDestroy(frontTrash, targetPosition));
    }

    public void removeTrashFromQueue(GameObject trash)
    {
        if (trashQueue.Count > 0 && trashQueue.Peek() == trash)
        {
            trashQueue.Dequeue();
            Destroy(trash);
        }
    }

    IEnumerator moveToBinAndDestroy(GameObject obj, Vector3 targetPosition)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 start = obj.transform.position;

        while (elapsed < duration)
        {
            obj.transform.position = Vector3.Lerp(start, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition;
        Destroy(obj);
    }

    Vector3 getTargetPosition(TrashType type)
    {
        switch (type)
        {
            case TrashType.Recyclable: return recycleLocator.GetTargetPosition();
            case TrashType.Compost: return compostLocator.GetTargetPosition();
            case TrashType.Fish: return fishLocator.GetTargetPosition();
            default: return Vector3.zero;
        }
    }
}