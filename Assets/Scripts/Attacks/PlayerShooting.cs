using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform firePoint; // Where bullet spawns

    // Subscribe to the channel when this object is enabled
    private void OnEnable()
    {
        InputBroadcaster.AttackEvent += Shoot;
    }

    // Unsubscribe when disabled (CRITICAL to prevent errors)
    private void OnDisable()
    {
        InputBroadcaster.AttackEvent -= Shoot;
    }

    private void Shoot()
    {
        // 1. Get a bullet from the pool
        GameObject bullet = BulletPool.Instance.GetBullet();

        if (bullet != null)
        {
            // 2. Set position and rotation to match the gun
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;

            // 3. Enable it (This triggers OnEnable in the Bullet script)
            bullet.SetActive(true);
        }
    }
}