using UnityEngine;
using UnityEngine.AI;

public class MovementAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    public Transform playerTransform;
    private Animator _animator;

    private static readonly int TargetFoundHash = Animator.StringToHash("targetFound");

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (_agent == null || playerTransform == null || !_agent.isOnNavMesh)
            return;

        if (Vector3.Distance(transform.position, playerTransform.position) < 100f)
        {
            _agent.SetDestination(playerTransform.position);
            if (_animator != null)
                _animator.SetBool(TargetFoundHash, true);
        }
        else if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            if (_animator != null)
                _animator.SetBool(TargetFoundHash, false);
        }
    }
}