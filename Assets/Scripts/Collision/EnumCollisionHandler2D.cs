using UnityEngine;

public class EnumCollisionHandler2D : MonoBehaviour
{
    private CollidableType myType;

    [Header("Possession – collision detection only")]
    public bool canPossess { get; private set; } = false;
    public GameObject CurrentPossessableNPC { get; private set; } = null;
    public NPCData CurrentPossessableNPCData { get; private set; } = null;
    private void Awake()
    {
        myType = GetComponent<CollidableType>();
        if(myType == null)
        {
            Debug.LogError("CollidableType component not found on " + gameObject.name);
            enabled = false;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // Physical collisions
    {

        var otherType = collision.collider.GetComponent<CollidableType>();
        if (otherType == null) return;

        // myType is based on object this script is attached to
        // otherType is based on the object we collided with

        switch (myType.types)
        {
            case CollidableTypes.PlayerMask:
                switch (otherType.types)
                {
                    case CollidableTypes.Projectile:
                        Debug.Log("Player hit by bullet! Take damage / knockback");
                        // TakeDamage();
                        break;

                    case CollidableTypes.NPC:
                        NPCData npcData = GetNPCData(collision.gameObject);
                        if (npcData != null)
                        {
                            CurrentPossessableNPCData = npcData;
                            CurrentPossessableNPC = collision.gameObject;
                            canPossess = true;
                            Debug.Log("Player bumped into NPC → maybe dialogue / push?");
                        }
                        break;

                    case CollidableTypes.Obstacle:
                        Debug.Log("Player ran into wall");
                        break;

                    case CollidableTypes.Melee:
                        Debug.Log("Player hit by melee attack!");
                        break;
                }
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var otherTypeComp = collision.collider.GetComponent<CollidableType>();
        if (otherTypeComp == null) return;

        if (otherTypeComp.types == CollidableTypes.NPC &&
            collision.gameObject == CurrentPossessableNPC)
        {
            canPossess = false;
            CurrentPossessableNPC = null;
            CurrentPossessableNPCData = null;
            Debug.Log("[Possession] Left NPC range");
        }
    }

    private NPCData GetNPCData(GameObject go)
    {
        var holder = go.GetComponent<NPCDataHolder>();
        if (holder != null)
        {
            return holder.Data;
        }

        // Optional fallbacks if you ever add it differently later
        return null;
    }

}
