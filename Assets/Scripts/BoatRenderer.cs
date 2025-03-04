using UnityEngine;

public class BoatRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private string currSprite;
    [SerializeField] bool docked;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Boats should be stored in Resources/Boats, the format is Boat_level(_Docked)
    public void Update()
    {
        string spriteName = "Boat_" + GameManager.Instance.boatUpgradeLevel + (docked ? "_Docked" : "");
        if (spriteName != currSprite)
        {
            currSprite = spriteName;
            Debug.Log("Loading Sprite: " + spriteName);
            spriteRenderer.sprite = Resources.Load<Sprite>("Boats/" + spriteName);
        }
    }
}
