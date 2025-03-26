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
}

public enum TrashType
{
    TRASH = 0,
    RECYCLE = 1,
    FISH = 2
}