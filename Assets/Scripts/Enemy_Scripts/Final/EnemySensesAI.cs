using UnityEngine;

public class EnemySensesAI : MonoBehaviour
{
    [Header("Eyes")]
    public float sightDistance = 20f;
    public float instantSpotDistance = 3f;
    public float fieldOfViewAngle = 110f;

    [Header("Ears and Alertness")]
    public float alertLevel = 0f;
    public float alertThreshold = 100f;

    [Header("Memory (Stealth)")]
    public float loseSightTimer = 1.5f;
    public float shadowThreshold = 20f;

    [Header("Layers")]
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [HideInInspector] public Vector3 lastKnownPosition;

    private Transform playerTransform;
    private PlayerStats playerStats;

    private bool canSeePlayer = false;
    private float timeSinceLastSeen = 0f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        ProcessVision();
        CoolDownAlert();
    }

    private void ProcessVision()
    {
        if (playerTransform == null) return;

        canSeePlayer = false;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= sightDistance)
        {
            Vector3 myEyes = transform.position + Vector3.up * 1.5f;
            Vector3 playerCenter = playerTransform.position + Vector3.up * 1.5f;

            Vector3 dirToPlayer = (playerCenter - myEyes).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < fieldOfViewAngle / 2f)
            {
                Debug.DrawRay(myEyes, dirToPlayer * distanceToPlayer, Color.yellow);

                if (!Physics.Raycast(myEyes, dirToPlayer, out RaycastHit hit, distanceToPlayer, obstacleMask))
                {
                    if (distanceToPlayer <= instantSpotDistance)
                    {
                        Debug.DrawRay(myEyes, dirToPlayer * distanceToPlayer, Color.red);
                        canSeePlayer = true;
                        timeSinceLastSeen = 0f;

                        alertLevel = alertThreshold;
                        lastKnownPosition = playerTransform.position;
                    }
                    else if (playerStats.visibility >= shadowThreshold)
                    {
                        Debug.DrawRay(myEyes, dirToPlayer * distanceToPlayer, Color.red);
                        canSeePlayer = true;
                        timeSinceLastSeen = 0f;

                        float visibilityMultiplier = playerStats.visibility / 100f;
                        float distanceFactor = 1f - (distanceToPlayer / sightDistance);

                        alertLevel += 30f * visibilityMultiplier * distanceFactor * Time.deltaTime;
                        alertLevel = Mathf.Clamp(alertLevel, 0, alertThreshold);

                        if (alertLevel > 10f)
                        {
                            lastKnownPosition = playerTransform.position;
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    public void OnHearNoise(Vector3 soundPosition, float noiseVolume)
    {
        alertLevel += noiseVolume;
        alertLevel = Mathf.Clamp(alertLevel, 0, alertThreshold);
        lastKnownPosition = soundPosition;
        Debug.Log($"Henry heard that. Alert level: {alertLevel}");
    }

    private void CoolDownAlert()
    {
        if (!canSeePlayer)
        {
            timeSinceLastSeen += Time.deltaTime;

            if (alertLevel >= alertThreshold)
            {
                float currentMemoryTarget = (playerStats.visibility < shadowThreshold) ? loseSightTimer : loseSightTimer * 4f;

                if (timeSinceLastSeen >= currentMemoryTarget)
                {
                    alertLevel = alertThreshold - 1f;
                    Debug.Log("Lost eye contact. Going to last known location.");
                }
            }
            else if (alertLevel > 0)
            {
                alertLevel -= 10f * Time.deltaTime;
                alertLevel = Mathf.Max(alertLevel, 0);
            }
        }
    }

    // ♫♫♫ Hermann methods ♫♫♫

    public bool IsTargetConfirmed()
    {
        return alertLevel >= alertThreshold;
    }

    public bool IsSuspicious()
    {
        return alertLevel > 0 && alertLevel < alertThreshold;
    }

    public Vector3 GetPointOfInterest()
    {
        return lastKnownPosition;
    }
}