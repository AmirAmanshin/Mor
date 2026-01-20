using UnityEngine;
using UnityEngine.AI;

public class GilbertAI : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private NavMeshAgent _agent;
    
    public Transform[] points;
    private int destPoint = 0;

    private void GoToNextPoint()
    {
        if (points.Length == 0) return;

        _agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;

        if (destPoint == 4)
        {
            destPoint = 0;
        }
    }

    void Start()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _agent = GetComponent<NavMeshAgent>();

        GoToNextPoint();
    }

    void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }
}
