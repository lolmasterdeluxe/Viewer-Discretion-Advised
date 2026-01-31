using UnityEngine;
using UnityEngine.InputSystem;

// Input system updated by Gemini

[RequireComponent(typeof(PlayerInput))]
public class PlayerPossession : MonoBehaviour
{
    [Header("Possession")]
    // keys are now handled by the Input System Asset, not here
    public GameObject possessionEffectPrefab;

    [Header("State")]
    [SerializeField] private bool isPossessing = false;

    private EnumCollisionHandler2D collisionHandler;
    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;

    // Original stats storage
    private float originalMaxHealth;
    private float originalMoveSpeed;
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

        SaveOriginalStats();
    }

    // ---------------------------------------------------------
    // NEW INPUT SYSTEM METHODS
    // These names must match your Input Actions (e.g., Action "Possess" -> OnPossess)
    // ---------------------------------------------------------

    public void OnPossess(InputValue value)
    {
        // Check if button is pressed down (value.isPressed) 
        // AND if we are allowed to possess
        if (value.isPressed && collisionHandler.canPossess)
        {
            PerformPossession();
        }
    }

    public void OnExorcise(InputValue value)
    {
        // Check if button is pressed AND we are currently possessing someone
        if (value.isPressed && isPossessing)
        {
            RevertToOriginalForm();
        }
    }

    // ---------------------------------------------------------

    private void SaveOriginalStats()
    {
        originalMaxHealth = playerStats.maxHealth;
        originalMoveSpeed = playerStats.moveSpeed;
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

        // Apply visuals
        if (spriteRenderer != null && npcData.sprite != null)
            spriteRenderer.sprite = npcData.sprite;

        if (possessionEffectPrefab != null)
            Instantiate(possessionEffectPrefab, transform.position, Quaternion.identity);

        // Remove the NPC from the world
        Destroy(npcObj);

        isPossessing = true;
    }

    private void RevertToOriginalForm()
    {
        Debug.Log("Exorcised – returned to original form");

        // Restore base stats
        playerStats.maxHealth = originalMaxHealth;
        playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, originalMaxHealth);
        playerStats.moveSpeed = originalMoveSpeed;

        // Restore appearance
        spriteRenderer.sprite = originalSprite;

        isPossessing = false;
    }
}