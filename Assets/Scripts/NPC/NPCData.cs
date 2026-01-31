using UnityEngine;

public abstract class NPCData : ScriptableObject
{
    [Header("Basic Identity")]
    public string npcName = "Unnamed Enemy";
    public string description = "";

    [Header("Visual")]
    public GameObject characterGameObject;

    [Header("Stats")]
    [Range(10f, 1000f)] public float maxHealth = 100f;
    [Range(0.5f, 8f)]   public float moveSpeed = 2.5f;

    [Header("Combat")]
    [Range(0f, 50f)]    public float attackDamage = 8f;
    [Range(0.3f, 3f)]   public float attackRate = 1f;
    [Range(0.5f, 8f)]   public float attackRange = 1.5f;

    public GameObject deathEffectPrefab;

    public abstract NPCType GetNPCType();


}

public enum NPCType
{
    Melee,
    Ranged
 
}

[CreateAssetMenu(fileName = "NPC_Melee", menuName = "NPC/NPC - Melee Basic", order = 100)]
public class NPCMeleeBasic : NPCData
{
    public override NPCType GetNPCType() => NPCType.Melee;

    private void OnValidate()
    {
        maxHealth = Mathf.Clamp(maxHealth, 80f, 180f);
        moveSpeed = Mathf.Clamp(moveSpeed, 1.8f, 3.2f);
        attackDamage = Mathf.Clamp(attackDamage, 10f, 22f);
        attackRange = 1.4f;     // typical melee range
        attackRate = 1.1f;
        
    }
}

[CreateAssetMenu(fileName = "NPC_Ranged", menuName = "NPC/NPC - Ranged Shooter", order = 101)]
public class NPCRangedShooter : NPCData
{
    [Header("Ranged Specific")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float minPreferredDistance = 4f;     // tries to keep this distance

    public override NPCType GetNPCType() => NPCType.Ranged;

    private void OnValidate()
    {
        maxHealth = Mathf.Clamp(maxHealth, 50f, 120f);
        moveSpeed = Mathf.Clamp(moveSpeed, 1.5f, 3.5f);
        attackDamage = Mathf.Clamp(attackDamage, 6f, 18f);
        attackRange = 7f;           // much larger
        attackRate = 1.3f;
        attackRange = Mathf.Max(attackRange, 5f);
    }
}