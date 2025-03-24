using UnityEngine;

[CreateAssetMenu(fileName = "Boat Type", menuName = "Game/Boat Type")]
public class BoatType : ScriptableObject
{
    public string Name;
    public int BoatLevel;
    public int NetLevel;
    public int InventoryCapacity;
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;

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
