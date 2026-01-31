using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private GameObject owner; // The specific player who shot this

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    // We add a new Setup function to pass data when spawning
    public void Initialize(GameObject shooter, Vector3 position, Quaternion rotation)
    {
        owner = shooter;

        transform.position = position;
        transform.rotation = rotation; // Use the spread rotation passed in

        gameObject.SetActive(true);

        // Reset velocity using the NEW rotation (this ensures it flies in the spread direction)
        rb.linearVelocity = transform.right * speed;

        Invoke(nameof(DisableBullet), lifeTime);
    }

    private void OnDisable() => CancelInvoke();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Owner: " + owner);

        // 1. IGNORE THE OWNER
        if (other.gameObject == owner) return; 

        // 2. IGNORE OTHER BULLETS (Optional, prevents mid-air collisions)
        if (other.GetComponent<Bullet>() != null) return;

        // 3. HIT LOGIC
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);
            DisableBullet();
        }
        else if (other.CompareTag("Obstacle"))
        {
            DisableBullet();
        }
        Debug.Log("Other: " + other);
    }

    private void DisableBullet() => gameObject.SetActive(false);
}