using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;  // Use only X position of this, Z will be randomized

    [Header("Z Spawn Range")]
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("Spawn Interval Range")]
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;

    private float nextSpawnTime;

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            ScheduleNextSpawn();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, Random.Range(minZ, maxZ));
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
