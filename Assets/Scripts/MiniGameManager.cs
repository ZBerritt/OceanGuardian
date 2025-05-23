using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text dabloons;
    [SerializeField] private TMP_Text itemsSorted;
    [SerializeField] private TMP_Text percentCorrect;
    [SerializeField] private TMP_Text percentCleaner;
    [SerializeField] private TMP_Text itemsReturning;
    //[SerializeField] private TMP_Text 
    public int numberOfItemsSorted = 0;
    public double correctPercentage = 0;
    public int itemsBackToOcean = 0;
    public int moneyEarned = 0;
    public double oceanCleanPercentage = 0;
    public int inventoryCount = 0;

    public BinTargetLocator otherRecyclableLocator;
    public BinTargetLocator trashLocator;
    public BinTargetLocator fishLocator;
    public BinTargetLocator PETELocator;
    public BinTargetLocator HDPELocator;
    public BinTargetLocator PVCLocator;
    public BinTargetLocator LDPELocator;
    public BinTargetLocator PPLocator;
    public BinTargetLocator PSLocator;
    public BinTargetLocator eWasteLocator;

    public Transform spawnPoint;
    public float spawnInterval = 1.5f;
    public float moveSpeed = 2f;

    private Queue<GameObject> trashQueue = new Queue<GameObject>();
    private float spawnTimer;
    private int activeTrashItems = 0; // Track all active trash items
    private bool gameEnded = false;

    void Start()
    {
        canvas.enabled = false;
        spawnTimer = spawnInterval;
        inventoryCount = GameManager.Instance.inventory.Count;
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
            // Update key bindings to match the specification
            if (Input.GetKeyDown(KeyCode.Alpha1)) handleBinInput(TrashType.PETE);
            if (Input.GetKeyDown(KeyCode.Alpha2)) handleBinInput(TrashType.HDPE);
            if (Input.GetKeyDown(KeyCode.Alpha3)) handleBinInput(TrashType.PVC);
            if (Input.GetKeyDown(KeyCode.Alpha4)) handleBinInput(TrashType.LDPE);
            if (Input.GetKeyDown(KeyCode.Alpha5)) handleBinInput(TrashType.PP);
            if (Input.GetKeyDown(KeyCode.Alpha6)) handleBinInput(TrashType.PS);
            if (Input.GetKeyDown(KeyCode.Alpha7)) handleBinInput(TrashType.EWaste);
            if (Input.GetKeyDown(KeyCode.Alpha8)) handleBinInput(TrashType.OtherRecyclable);
            if (Input.GetKeyDown(KeyCode.Alpha9)) handleBinInput(TrashType.Trash);
            if (Input.GetKeyDown(KeyCode.Alpha0)) handleBinInput(TrashType.Fish);
        }

        // Show the end screen only when inventory is empty AND there are no active trash items
        if (!gameEnded && GameManager.Instance.inventory.Count == 0 && activeTrashItems == 0 && trashQueue.Count == 0)
        {
            ShowEndScreen();
            UpdateGameState();
        }
    }

    void ShowEndScreen()
    {
        gameEnded = true;
        canvas.enabled = true;
        itemsSorted.text = numberOfItemsSorted + " out of " + inventoryCount + " Items were sorted";
        itemsReturning.text = itemsBackToOcean + " Items Returning to Ocean";
        percentCorrect.text = correctPercentage.ToString("F1") + "% of trash was sorted correctly";
        percentCleaner.text = "The ocean was made " + oceanCleanPercentage.ToString("F1") + "% cleaner";
        dabloons.text = moneyEarned + " Doubloons Earned";
    }

    void UpdateGameState()
    {
        GameManager.Instance.doubloons += moneyEarned;
        GameManager.Instance.trashDensity = (int) Mathf.Max(0f, GameManager.Instance.trashDensity - math.ceil(numberOfItemsSorted / 10f));
    }

    public void EndEvening()
    {
        GameManager.Instance.day++;
        GameManager.Instance.timeOfDay = TimeOfDay.Morning;
        SceneManager.LoadScene("TrashFacilityScene");
    }

    void handleBinInput(TrashType selectedType)
    {
        if (trashQueue.Count == 0) return;

        GameObject frontTrash = trashQueue.Dequeue();
        TrashItem item = frontTrash.GetComponent<TrashItem>();

        // Update the number of items sorted
        numberOfItemsSorted++;

        // Scoring
        if (item.data.Type == selectedType)
        {
            // Correct sorting - add doubloons based on the type
            switch (selectedType)
            {
                case TrashType.PETE:
                case TrashType.PP:
                case TrashType.OtherRecyclable:
                    moneyEarned += 2;
                    break;
                case TrashType.HDPE:
                case TrashType.PVC:
                case TrashType.LDPE:
                case TrashType.PS:
                case TrashType.Trash:
                    moneyEarned += 1;
                    break;
                case TrashType.EWaste:
                    moneyEarned += 3;
                    break;
                case TrashType.Fish:
                    // No doubloons for fish
                    break;
            }
        }

        // Calculate percentages after each item
        int totalPossibleDoubloons = 0;
        // This should be calculated based on your inventory's actual contents
        // For now, assuming an average of 2 doubloons per correct item as a placeholder
        for (int i = 0; i < inventoryCount; i++)
        {
            // You would need to access the actual inventory item types here
            // For this example, assuming all items could be worth 2 doubloons when correctly sorted
            totalPossibleDoubloons += 2;
        }

        correctPercentage = ((double)moneyEarned / totalPossibleDoubloons) * 100;

        // Items returning to ocean = total inventory - sorted items
        itemsBackToOcean = inventoryCount - numberOfItemsSorted;

        // Calculate ocean clean percentage
        oceanCleanPercentage = ((double)numberOfItemsSorted / (GameManager.Instance.trashDensity * 10)) * 100;

        Vector3 targetPosition = getTargetPosition(selectedType);
        Destroy(frontTrash.GetComponent<TrashMove>());
        StartCoroutine(moveToBinAndDestroy(frontTrash, targetPosition));
    }

    void SpawnTrash()
    {
        TrashItemData data = GameManager.Instance.inventory[0];
        GameManager.Instance.inventory.RemoveAt(0);

        GameObject obj = TrashItem.Spawn(trashPrefab, spawnPoint.position, data);
        obj.GetComponent<SpriteRenderer>().sortingOrder = 2; // Force on top
        obj.transform.localScale = new Vector3(0.32f, 0.32f, 1f);
        TrashMove mover = obj.AddComponent<TrashMove>();
        mover.Init(moveSpeed, this);

        trashQueue.Enqueue(obj);
        activeTrashItems++; // Increment active trash count
    }

    public void removeTrashFromQueue(GameObject trash)
    {
        if (trashQueue.Count > 0 && trashQueue.Peek() == trash)
        {
            trashQueue.Dequeue();
            Destroy(trash);
            activeTrashItems--; // Decrement active trash count
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
        activeTrashItems--; // Decrement active trash count when item is destroyed
    }

    Vector3 getTargetPosition(TrashType type)
    {
        switch (type)
        {
            // You'll need to add more BinTargetLocator variables for each bin type
            // For now, I'll map to existing ones as placeholders
            case TrashType.PETE:
                return PETELocator.GetTargetPosition();
            case TrashType.HDPE:
                return HDPELocator.GetTargetPosition();
            case TrashType.PVC:
                return PVCLocator.GetTargetPosition();
            case TrashType.LDPE:
                return LDPELocator.GetTargetPosition();
            case TrashType.PP:
                return PPLocator.GetTargetPosition();
            case TrashType.PS:
                return PSLocator.GetTargetPosition();
            case TrashType.EWaste:
                return eWasteLocator.GetTargetPosition();
            case TrashType.OtherRecyclable:
                return otherRecyclableLocator.GetTargetPosition();
            case TrashType.Trash:
                return trashLocator.GetTargetPosition();
            case TrashType.Fish:
                return fishLocator.GetTargetPosition();
            default:
                return Vector3.zero;
        }
    }
}