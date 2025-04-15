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

public enum TrashType { Recyclable, Compost, Fish }