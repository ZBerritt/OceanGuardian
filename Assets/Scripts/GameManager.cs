using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Add any sort of save data here
    public int boatUpgradeLevel = 0;
    public int boatNetLevel = 0;
    public Vector2 playerPosition;
    public int trashDensity = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
