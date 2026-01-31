using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance; // Singleton for easy access

    [Header("Pool Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 10;

    // The actual pool list
    private List<GameObject> pooledBullets = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.SetActive(false);
        bullet.transform.SetParent(transform); // Keep Hierarchy clean
        pooledBullets.Add(bullet);
        return bullet;
    }

    public GameObject GetBullet()
    {
        // 1. Search for an inactive bullet in the list
        foreach (GameObject bullet in pooledBullets)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        // 2. If no bullets are free, create a new one (expand the pool)
        return CreateNewBullet();
    }
}