using UnityEngine;


public enum CollidableTypes //Add tags here, if more is needed
{
    None,
    PlayerMask,
    NPC,
    Obstacle,
    Projectile,
    Melee
}

public class CollidableObject2D : MonoBehaviour //Set the types of the object in the inspector
{
    [Header("Collision Type")]
    public CollidableTypes types = CollidableTypes.None;
}
