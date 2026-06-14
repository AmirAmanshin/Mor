using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private PlayerUI _playerUI;

    [Header("Wave Settings")]

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        SpawnWave();
    }

    void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        if (activeEnemies.Count == 0)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {

       
    }

    private void SpawnSingleEnemy()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogWarning("No place for spawning.");
        }
    }
}