using NUnit.Framework;
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
    [UnityEngine.Range(0f, 45f)] // Adds a slider in the Inspector
    [SerializeField] private float spreadAngle = 5f;

    [Header("Juice / VFX")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Sprite[] muzzleFlashes;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.15f;
    [SerializeField] private float flashDuration = 0.1f; // Duration in seconds
    [SerializeField] private float timer;

    [Header("Physics")]
    [Tooltip("How hard the gun kicks back. Try values between 2 and 10.")]
    [SerializeField] private float recoilForce = 5f;

    private Rigidbody2D rb; // Reference to the physics body

    private bool isTriggerHeld = false;
    private float nextFireTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() => InputBroadcaster.AttackEvent += HandleFireInput;
    private void OnDisable() => InputBroadcaster.AttackEvent -= HandleFireInput;

    private void HandleFireInput(bool isPressed)
    {
        isTriggerHeld = isPressed;
        if (fireMode == FireMode.SemiAuto && isTriggerHeld)
            AttemptShoot();
    }

    private void Update()
    {
        if (fireMode == FireMode.Automatic && isTriggerHeld)
            AttemptShoot();

        // Update the timer
        timer += Time.deltaTime;

        // Deactivate the flash if the duration is over and it's active
        if (timer > flashDuration && muzzleFlash.activeSelf)
            muzzleFlash.SetActive(false);
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
            // 1. Calculate Spread FIRST
            float randomZAngle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, randomZAngle);

            // Combine gun direction with spread
            Quaternion finalRotation = firePoint.rotation * spreadRotation;

            // 2. Get the script
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // 3. Initialize with the CALCULATED rotation
            bulletScript.Initialize(gameObject, firePoint.position, finalRotation);

            // 2. Play Muzzle Flash (If assigned)
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
                muzzleFlash.GetComponent<SpriteRenderer>().sprite = muzzleFlashes[Random.Range(0, muzzleFlashes.Length)];
                timer = 0f; // Reset the timer
            }

            // 3. Trigger Screen Shake
            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);

            // Update the Recoil section in Shoot()
            if (rb != null)
            {
                // Get the player controller script
                var controller = GetComponent<PlayerController>(); // Or whatever your script is named

                if (controller != null)
                {
                    // Apply force and disable movement for 0.1 seconds
                    controller.ApplyRecoil(-firePoint.right * recoilForce, 0.1f);
                }
            }
        }
    }
}