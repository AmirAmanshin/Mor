using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject currentEnemy;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPointTransform;

    

    void Start()
    {
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    void Update()
    {
        if (currentEnemy == null)
        {
            
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    { 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPointTransform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            currentEnemy = Instantiate(enemyPrefab, hit.position, spawnPointTransform.rotation);
        }
    }
}