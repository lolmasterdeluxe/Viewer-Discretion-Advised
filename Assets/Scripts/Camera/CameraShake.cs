using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton for global access

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines(); // Stop any previous shake so they don't fight
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Pick a random point inside a small circle
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        transform.localPosition = originalPos; // Reset perfectly
    }
}