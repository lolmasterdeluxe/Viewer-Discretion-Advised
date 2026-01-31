using System.ComponentModel;
using UnityEngine;

// Input system updated by Gemini
public class PlayerPossession : MonoBehaviour
{
    [Header("Possession Settings")]
    [SerializeField] private float possessRange = 3f;
    [SerializeField] private LayerMask npcLayer; // Set this to "NPC" in Inspector
    public GameObject possessionEffectPrefab;

    [Header("State")]
    [SerializeField] private bool isPossessing = false;

    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;

    [SerializeField]
    private GameObject playerCharacter;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    // ---------------------------------------------------------
    // NEW INPUT SYSTEM METHODS
    // These names must match your Input Actions (e.g., Action "Possess" -> OnPossess)
    // ---------------------------------------------------------

    // Subscribe to the channel when this object is enabled
    private void OnEnable()
    {
        InputBroadcaster.PossessEvent += TryPossess;
        InputBroadcaster.ExorciseEvent += TryExorcise;
    }

    private void OnDisable()
    {
        InputBroadcaster.PossessEvent -= TryPossess;
        InputBroadcaster.ExorciseEvent -= TryExorcise;
    }

    public void TryPossess()
    {
        if (isPossessing) return;

        // 1. SCAN FOR ENEMIES
        // This works even if Physics Matrix collision is disabled!
        Collider2D hit = Physics2D.OverlapCircle(transform.position, possessRange, npcLayer);

        if (hit != null)
        {
            NPCDataHolder dataHolder = hit.gameObject.GetComponent<NPCDataHolder>();

            if (dataHolder.Data != null)
            {
                PerformPossession(dataHolder.Data, hit.gameObject);
            }
        }
    }

    private void TryExorcise()
    {
        if (isPossessing) RevertToOriginalForm();
    }

    // DEBUG: Draw the range in Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, possessRange);
    }

    // ---------------------------------------------------------

    private void PerformPossession(NPCData npcData, GameObject npcObj)
    {
        SoundManager.Instance.PlaySFX("SFX/FightPhaseSFX/Possessing");
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
        collider.enabled = false;
    }

    private void RevertToOriginalForm()
    {
        Debug.Log("Exorcised – returned to original form");

        // Restore appearance
        spriteRenderer.enabled = true;
        collider.enabled = true;
        Destroy(playerCharacter);

        isPossessing = false;
    }
}



