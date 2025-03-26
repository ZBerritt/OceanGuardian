using System.Collections.Generic;
using UnityEngine;

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
