using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Boat Type", menuName = "Game/Boat Type")]
public class BoatType : ScriptableObject
{
    public string Name;
    public int BoatLevel;
    public int NetLevel;
    public int InventoryCapacity;
    public int MaxSpeed;
    public int Acceleration;
    public int Deceleration;

    [Header("Sprites")]
    public Sprite northSprite;
    public Sprite eastSprite;
    public Sprite southSprite;
    public Sprite westSprite;
    public Sprite dockedSprite;

    [Header("Animations (not implemented)")]
    public Animator northAnimator;
    public Animator southAnimator;
    public Animator eastAnimator;
    public Animator westAnimator;
}

[CreateAssetMenu(fileName = "Boat Database", menuName = "Game/Boat Database")]
public class BoatDatabase : ScriptableObject
{
    public List<BoatType> boatTypes;

    // Get the exact boat type that matches both boat level and net level
    public BoatType GetBoatType(int boatLevel, int netLevel)
    {
        List<BoatType> validBoats = boatTypes.FindAll(b => b.BoatLevel == boatLevel);

        if (validBoats.Count == 0)
            return null;

        if (validBoats.Count == 1)
            return validBoats[0];

        return validBoats.Find(b => b.NetLevel == netLevel);
    }
}
