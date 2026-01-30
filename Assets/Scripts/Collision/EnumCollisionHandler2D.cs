using UnityEngine;

public class EnumCollisionHandler2D : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) // Physical collisions
    {
        CollidableObject2D collidable = collision.collider.GetComponent<CollidableObject2D>();
        if(collidable != null)
        {
            switch (collidable.types)
            {
                case CollidableTypes.PlayerMask:
                    Debug.Log("Collided with PlayerMask");
                    //Add custom behaviour here
                    break;

                case CollidableTypes.NPC:
                    Debug.Log("Collided with NPC");
                    //Add custom behaviour here
                    break;
               
                case CollidableTypes.Obstacle:
                    Debug.Log("Collided with Obstacle");
                    //Add custom behaviour here
                    break;
                
                case CollidableTypes.Projectile:
                    Debug.Log("Collided with Projectile");
                    //Add custom behaviour here
                    break;
                
                case CollidableTypes.Melee:
                    Debug.Log("Collided with Melee Attack");
                    //Add custom behaviour here 
                break;
                default:
                    Debug.Log("Collided with Unknown Type");
                    break;
            }
        }
    }

}
