using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public enum FireMode { SemiAuto, Automatic }

    [Header("General Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private FireMode fireMode = FireMode.Automatic;
    [SerializeField] private float fireRate = 5f;

    [Header("Accuracy")]
    [Tooltip("Total angle of variance in degrees. Higher = less accurate.")]
    [Range(0f, 45f)] // Adds a slider in the Inspector
    [SerializeField] private float spreadAngle = 5f;

    private bool isTriggerHeld = false;
    private float nextFireTime = 0f;

    private void OnEnable()
    {
        InputBroadcaster.AttackEvent += HandleFireInput;
    }

    private void OnDisable()
    {
        InputBroadcaster.AttackEvent -= HandleFireInput;
    }

    private void HandleFireInput(bool isPressed)
    {
        isTriggerHeld = isPressed;
        if (fireMode == FireMode.SemiAuto && isTriggerHeld)
        {
            AttemptShoot();
        }
    }

    private void Update()
    {
        if (fireMode == FireMode.Automatic && isTriggerHeld)
        {
            AttemptShoot();
        }
    }

    private void AttemptShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    // ----------- NEW SHOOT LOGIC -----------
    private void Shoot()
    {
        GameObject bullet = BulletPool.Instance.GetBullet();

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;

            // 1. Calculate random spread variance
            // We divide by 2 so a spread of "10" means +/- 5 degrees left and right
            float randomZAngle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);

            // 2. Create a rotation from that angle (Z-axis for 2D)
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, randomZAngle);

            // 3. Apply the spread to the current direction
            // Note: Multiplying Quaternions adds their rotations together
            Quaternion finalRotation = firePoint.rotation * spreadRotation;

            bullet.transform.rotation = finalRotation;

            bullet.SetActive(true);
        }
    }
}