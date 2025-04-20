using UnityEngine;

[CreateAssetMenu(fileName = "TrashItem", menuName = "Game/Trash Item")]
public class TrashItemData : ScriptableObject
{
    public TrashType Type;
    public string Name;
    public Sprite ItemSprite;
    [TextArea(2, 5)]
    public string Description;
    public int Score;
    public bool Big;
    public GameObject prefab;
}

public enum TrashType
{
    PETE,       // Key 1
    HDPE,       // Key 2
    PVC,        // Key 3
    LDPE,       // Key 4
    PP,         // Key 5
    PS,         // Key 6
    EWaste,     // Key 7
    OtherRecyclable, // Key 8 (Glass/Aluminum/Other)
    Trash,      // Key 9
    Fish        // Key 0
}