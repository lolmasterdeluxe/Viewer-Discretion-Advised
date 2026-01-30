using UnityEngine;


public enum CollidableTypes //Add tags here, if more is needed
{
    None,
    Player,
    Enemy,
    Obstacle,
    PowerUp
}

public class CollidableObject2D : MonoBehaviour //Set the types of the object in the inspector
{
    [Header("Collision Type")]
    public CollidableTypes types = CollidableTypes.None;
}
