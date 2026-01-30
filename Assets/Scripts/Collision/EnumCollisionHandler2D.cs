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
                case CollidableTypes.Player:
                    Debug.Log("Collided with Player");
                    //Add custom behaviour here
                    break;

                case CollidableTypes.Enemy:
                    Debug.Log("Collided with Enemy");
                    //Add custom behaviour here
                    break;
               
                case CollidableTypes.Obstacle:
                    Debug.Log("Collided with Obstacle");
                    //Add custom behaviour here
                    break;
                
                case CollidableTypes.PowerUp:
                    Debug.Log("Collided with PowerUp");
                    //Add custom behaviour here
                    break;

                default:
                    Debug.Log("Collided with Unknown Type");
                    break;
            }
        }
    }

}
