using System;
using UnityEngine;

public enum TrashType
{
    TRASH,
    RECYCLE,
    FISH
}

public class TrashData : ScriptableObject
{
    public TrashType Type;
    public string Name;
    public string Description;
    public int Score;

    public TrashData(TrashType type, string name, string description, int score)
    {
        Type = type;
        Name = name;
        Description = description;
        Score = score;
    }

    public static TrashData GenerateRandomTrash()
    {
        TrashType type = (TrashType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TrashType)).Length);
        int score = UnityEngine.Random.Range(1, 6);
        return new TrashData(type, "placeholder name", "placeholder desc", score);
    }
}