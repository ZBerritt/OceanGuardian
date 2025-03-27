using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashDatabase", menuName = "Game/Trash Database")]
public class TrashDatabase : ScriptableObject
{
    [SerializeField] private List<TrashItemData> items = new();

    public TrashItemData GetRandomItem()
    {
        return items[UnityEngine.Random.Range(0, items.Count)];
    }

#if UNITY_EDITOR
    public void AddItem(TrashItemData item)
    {
        items.Add(item);
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

