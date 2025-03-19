using System;
using System.Collections.Generic;
using UnityEngine;

public enum TrashType
{
    TRASH = 0,
    RECYCLE = 1,
    FISH = 2
}

[CreateAssetMenu(fileName = "TrashDatabase", menuName = "Game/Trash Database")]
public class TrashDatabase : ScriptableObject
{
    [SerializeField] private List<TrashItemData> trashItems = new();
    [SerializeField] private List<TrashItemData> recycleItems = new();
    [SerializeField] private List<TrashItemData> fishItems = new();

    public TrashItemData GetRandomItem(TrashType type)
    {
        List<TrashItemData> itemList;

        switch (type)
        {
            case TrashType.RECYCLE:
                itemList = recycleItems;
                break;
            case TrashType.FISH:
                itemList = fishItems;
                break;
            case TrashType.TRASH:
            default:
                itemList = trashItems;
                break;
        }

        if (itemList.Count == 0)
            return null;

        return itemList[UnityEngine.Random.Range(0, itemList.Count)];
    }

    public TrashItemData GetRandomItem()
    {
        TrashType randomType = (TrashType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TrashType)).Length);
        return GetRandomItem(randomType);
    }

#if UNITY_EDITOR
    public void AddItem(TrashItemData item)
    {
        switch (item.Type)
        {
            case TrashType.RECYCLE:
                recycleItems.Add(item);
                break;
            case TrashType.FISH:
                fishItems.Add(item);
                break;
            case TrashType.TRASH:
            default:
                trashItems.Add(item);
                break;
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

[CreateAssetMenu(fileName = "TrashItem", menuName = "Game/Trash Item")]
public class TrashItemData : ScriptableObject
{
    public TrashType Type;
    public string Name;
    public Sprite ItemSprite;
    [TextArea(2, 5)]
    public string Description;
    public int Score;
}