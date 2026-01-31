using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float moveSpeed = 5f;
    public float attackDamage = 10f;
    public float attackRate = 1f;
    public float attackRange = 1.5f;

    // Add more stats as needed (e.g. projectilePrefab for ranged, etc.)
}
