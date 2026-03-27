using UnityEngine;
using UnityEngine.AI;

public class MovementAI : MonoBehaviour
{
    // TODO: privyazat' skorost' is "FightingAI" k skorosti dvizheniya.
    private FightingAI _fightingAI;
    private NavMeshAgent _agent;
    public Transform playerTransform;

    private int destPoint = 0;

    private void GoToNextPoint()
    {
        //if (points.Length == 0) return;

        //_agent.destination = points[destPoint].position;
        //destPoint = (destPoint + 1) % points.Length;

        //if (destPoint == 4)
        //{
        //    destPoint = 0;
        //}
    }

    void ChasePlayer()
    {
        _agent.SetDestination(playerTransform.position);
    }

    void Start()
    {
        _fightingAI = GetComponent<FightingAI>();
        _agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        GoToNextPoint();
    }

    void Update()
    {
        if (_agent != null)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < 10f)
            {
                ChasePlayer();
            }
            else if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                GoToNextPoint();
            }
        }
    }
}
