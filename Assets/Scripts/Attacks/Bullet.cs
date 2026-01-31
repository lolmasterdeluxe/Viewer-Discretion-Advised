using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // OnEnable is called every time the bullet comes out of the pool
    private void OnEnable()
    {
        // 1. Reset Velocity (In case it had leftover momentum)
        rb.linearVelocity = transform.right * speed; // Use transform.up if your sprite faces up

        // 2. Start the "Disable" timer
        Invoke(nameof(DisableBullet), lifeTime);
    }

    private void OnDisable()
    {
        // Cancel the timer so it doesn't trigger if we re-enable quickly
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Example collision logic
        if (other.CompareTag("Enemy"))
        {
            // other.GetComponent<EnemyHealth>().TakeDamage(damage);
            DisableBullet(); // Hide bullet on impact
        }
        else if (other.CompareTag("Wall"))
        {
            DisableBullet();
        }
        DisableBullet();
    }

    private void DisableBullet()
    {
        gameObject.SetActive(false); // Return to "pool" effectively
    }
}