using System.ComponentModel;
using UnityEngine;

// Input system updated by Gemini
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

    [SerializeField]
    private GameObject playerCharacter;

    private void Awake()
    {
        collisionHandler = GetComponent<EnumCollisionHandler2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (collisionHandler == null)
        {
            Debug.LogError("EnumCollisionHandler2D missing on player", this);
            enabled = false;
        }
    }

    // ---------------------------------------------------------
    // NEW INPUT SYSTEM METHODS
    // These names must match your Input Actions (e.g., Action "Possess" -> OnPossess)
    // ---------------------------------------------------------

    // Subscribe to the channel when this object is enabled
    private void OnEnable()
    {
        InputBroadcaster.PossessEvent += OnPossess;
        InputBroadcaster.ExorciseEvent += OnExorcise;
    }

    // Unsubscribe when disabled (CRITICAL to prevent errors)
    private void OnDisable()
    {
        InputBroadcaster.PossessEvent -= OnPossess;
        InputBroadcaster.ExorciseEvent -= OnExorcise;
    }

    public void OnPossess()
    {
        if (collisionHandler.canPossess)
            PerformPossession();
    }

    public void OnExorcise()
    {
        if (isPossessing)
            RevertToOriginalForm();
    }

    // ---------------------------------------------------------

    private void PerformPossession()
    {
        var npcData = collisionHandler.CurrentPossessableNPCData;
        var npcObj = collisionHandler.CurrentPossessableNPC;

        if (npcData == null || npcObj == null) return;

        Debug.Log($"Possessed → {npcData.npcName}");

        // Change to player game object
        if (gameObject != null && npcData.characterGameObject != null)
        {
            playerCharacter = Instantiate(npcData.characterGameObject, transform.position, Quaternion.identity);
            playerStats = playerCharacter.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // Apply stats
                playerStats.maxHealth = npcData.maxHealth;
                playerStats.currentHealth = npcData.maxHealth;
                playerStats.moveSpeed = npcData.moveSpeed;
            }
        }

        if (possessionEffectPrefab != null)
            Instantiate(possessionEffectPrefab, transform.position, Quaternion.identity);

        // Remove the NPC from the world
        Destroy(npcObj);

        isPossessing = true;

        spriteRenderer.enabled = false;
    }

    private void RevertToOriginalForm()
    {
        Debug.Log("Exorcised – returned to original form");

        // Restore appearance
        spriteRenderer.enabled = true;
        Destroy(playerCharacter);

        isPossessing = false;
    }
}