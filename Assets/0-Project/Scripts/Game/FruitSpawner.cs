using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] fruitPrefabs;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnIntervalVariance = 0.3f;
    [SerializeField] private int maxActiveObjects = 10;
    
    [Header("Spawn Probability")]
    [SerializeField] [Range(0f, 1f)] private float bombSpawnChance = 0.2f;
    
    [Header("Launch Settings")]
    [SerializeField] private float launchForceMin = 10f;
    [SerializeField] private float launchForceMax = 15f;
    [SerializeField] private float launchAngleMin = 60f;
    [SerializeField] private float launchAngleMax = 120f;
    [SerializeField] private float gravityScale = 1f;
    
    [Header("Spawn Position")]
    [SerializeField] private float spawnYPosition = -6f;
    [SerializeField] private float spawnXMin = -7f;
    [SerializeField] private float spawnXMax = 7f;
    
    [Header("Spawn Patterns")]
    [SerializeField] private bool enableMultiSpawn = true;
    [SerializeField] private float multiSpawnChance = 0.3f;
    [SerializeField] private int maxSimultaneousSpawn = 3;
    
    private FruitNinjaManager gameManager;
    private bool isSpawning = false;
    private int activeObjectCount = 0;
    private Coroutine spawnCoroutine;
    
    public void Initialize(FruitNinjaManager manager)
    {
        gameManager = manager;
    }
    
    public void StartSpawning()
    {
        if (isSpawning) return;
        
        isSpawning = true;
        activeObjectCount = 0;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            // Wait for spawn interval with variance
            float waitTime = spawnInterval + Random.Range(-spawnIntervalVariance, spawnIntervalVariance);
            yield return new WaitForSeconds(Mathf.Max(0.1f, waitTime));
            
            // Check if we can spawn more objects
            if (activeObjectCount >= maxActiveObjects)
                continue;
            
            // Decide single or multi spawn
            if (enableMultiSpawn && Random.value < multiSpawnChance)
            {
                SpawnMultipleObjects();
            }
            else
            {
                SpawnSingleObject();
            }
        }
    }
    
    private void SpawnSingleObject()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        SpawnObject(spawnPos);
    }
    
    private void SpawnMultipleObjects()
    {
        int spawnCount = Random.Range(2, maxSimultaneousSpawn + 1);
        
        for (int i = 0; i < spawnCount; i++)
        {
            if (activeObjectCount >= maxActiveObjects)
                break;
            
            Vector3 spawnPos = GetRandomSpawnPosition();
            SpawnObject(spawnPos);
            
            // Small delay between multi-spawns for visual variation
            StartCoroutine(DelayedSpawn(spawnPos, i * 0.1f));
        }
    }
    
    private IEnumerator DelayedSpawn(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Spawn is already handled in SpawnMultipleObjects, this is just for timing
    }
    
    private void SpawnObject(Vector3 position)
    {
        GameObject prefabToSpawn;
        bool isBomb = Random.value < bombSpawnChance;
        
        if (isBomb && bombPrefab != null)
        {
            prefabToSpawn = bombPrefab;
        }
        else if (fruitPrefabs != null && fruitPrefabs.Length > 0)
        {
            prefabToSpawn = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        }
        else
        {
            Debug.LogWarning("No fruit prefabs assigned to spawner!");
            return;
        }
        
        GameObject spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
        
        // Setup physics
        Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = gravityScale;
        
        // Apply launch force
        float launchAngle = Random.Range(launchAngleMin, launchAngleMax);
        float launchForce = Random.Range(launchForceMin, launchForceMax);
        
        // Calculate launch direction with X variance
        // Objects from left side will arc to the right, and vice versa
        float horizontalVariance = Random.Range(-0.3f, 0.3f);
        
        // Add natural arc based on spawn position
        if (position.x < 0)
        {
            // From left, add rightward bias
            horizontalVariance += Random.Range(0.1f, 0.4f);
        }
        else if (position.x > 0)
        {
            // From right, add leftward bias
            horizontalVariance -= Random.Range(0.1f, 0.4f);
        }
        
        Vector2 launchDirection = new Vector2(
            Mathf.Cos(launchAngle * Mathf.Deg2Rad) + horizontalVariance,
            Mathf.Sin(launchAngle * Mathf.Deg2Rad)
        ).normalized;
        
        rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        
        // Add slight rotation
        rb.angularVelocity = Random.Range(-180f, 180f);
        
        // Setup sliceable component
        SliceableObject sliceable = spawnedObject.GetComponent<SliceableObject>();
        if (sliceable == null)
        {
            sliceable = spawnedObject.AddComponent<SliceableObject>();
        }
        
        sliceable.Initialize(gameManager);
        sliceable.SetObjectType(isBomb ? SliceableObject.ObjectType.Bomb : SliceableObject.ObjectType.Fruit);
        
        // Track active objects
        activeObjectCount++;
        StartCoroutine(TrackObjectLifetime(spawnedObject));
    }
    
    private IEnumerator TrackObjectLifetime(GameObject obj)
    {
        // Wait until object is destroyed
        while (obj != null)
        {
            yield return null;
        }
        
        activeObjectCount--;
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnXMin, spawnXMax);
        return new Vector3(randomX, spawnYPosition, 0f);
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize spawn area
        Gizmos.color = Color.green;
        Vector3 leftPoint = new Vector3(spawnXMin, spawnYPosition, 0);
        Vector3 rightPoint = new Vector3(spawnXMax, spawnYPosition, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);
    }
}
