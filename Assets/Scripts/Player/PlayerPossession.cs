using UnityEngine;
// Script to be attached to playerobject to handle possession mechanics
public class PlayerPossession : MonoBehaviour
{
    [Header("Possession")]
    public KeyCode possessKey = KeyCode.E;
    public GameObject possessionEffectPrefab;

    [Header("State")]
    [SerializeField] private bool isPossessing = false;           // are we currently an NPC?
    [SerializeField] private KeyCode exorciseKey = KeyCode.Q;     // optional: return to human form

    private EnumCollisionHandler2D collisionHandler;
    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;


    // Optional: remember original stats / appearance so we can revert
    private float originalMaxHealth;
    private float originalMoveSpeed;
    // ... add other base stats you want to restore
    private Sprite originalSprite;
    private void Awake()
    {
        collisionHandler = GetComponent<EnumCollisionHandler2D>();
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (collisionHandler == null)
        {
            Debug.LogError("EnumCollisionHandler2D missing on player", this);
            enabled = false;
        }

        // Remember original ("human") values once at start
        SaveOriginalStats();
    }

    private void Update()
    {
        if (collisionHandler.canPossess && Input.GetKeyDown(possessKey))
        {
            PerformPossession();
        }

        // Optional: allow player to leave possessed body
        if (isPossessing && Input.GetKeyDown(exorciseKey))
        {
            RevertToOriginalForm();
        }
    }

    private void SaveOriginalStats()
    {
        originalMaxHealth = playerStats.maxHealth;
        originalMoveSpeed = playerStats.moveSpeed;
        // originalAttackDamage = playerStats.attackDamage;  // etc.
        originalSprite = spriteRenderer.sprite;
    }

    private void PerformPossession()
    {
        var npcData = collisionHandler.CurrentPossessableNPCData;
        var npcObj = collisionHandler.CurrentPossessableNPC;

        if (npcData == null || npcObj == null) return;

        Debug.Log($"Possessed → {npcData.npcName}");

        // Apply stats
        playerStats.maxHealth = npcData.maxHealth;
        playerStats.currentHealth = npcData.maxHealth;
        playerStats.moveSpeed = npcData.moveSpeed;
        playerStats.attackDamage = npcData.attackDamage;
        playerStats.attackRate = npcData.attackRate;
        playerStats.attackRange = npcData.attackRange;

        // Optional visuals
        if (spriteRenderer != null && npcData.sprite != null)
            spriteRenderer.sprite = npcData.sprite;

        if (possessionEffectPrefab != null)
            Instantiate(possessionEffectPrefab, transform.position, Quaternion.identity);

        // Remove original NPC
        Destroy(npcObj);

        isPossessing = true;

    }
    private void RevertToOriginalForm()
    {
        Debug.Log("Exorcised – returned to original form");

        // Restore base stats
        playerStats.maxHealth = originalMaxHealth;
        playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, originalMaxHealth); // don't overheal
        playerStats.moveSpeed = originalMoveSpeed;
        // restore other stats you saved...

        // Restore appearance
        spriteRenderer.sprite = originalSprite;

        // Optional: spawn effect, sound, etc.

        isPossessing = false;
    }
}
