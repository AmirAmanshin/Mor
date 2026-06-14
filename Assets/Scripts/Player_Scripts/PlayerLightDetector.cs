using Unity.VisualScripting;
using UnityEngine;

public class PlayerLightDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Tooltip("Point from where the light detection rays are cast")]
    [SerializeField] private Transform checkPoint;

    [Header("Settings")]
    [Tooltip("Layers that block light")]
    [SerializeField] private LayerMask obstacleLayer;

    [Tooltip("How often to update the light calculation. 0.1f = 10 times per second.")]
    [SerializeField] private float checkInterval = 0.1f;

    [Tooltip("Base light level in the shadows")]
    [SerializeField] private float ambientLight = 0.1f;

    private Light[] _allLights;

    private void Start()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (checkPoint == null)
            checkPoint = transform;

        _allLights = FindObjectsByType<Light>();

        InvokeRepeating(nameof(CalculateIllumination), 0f, checkInterval);
    }

    private void CalculateIllumination()
    {
        float totalIllumination = ambientLight;

        foreach (Light light in _allLights)
        {
            if (!light.isActiveAndEnabled) continue;

            if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                float distance = Vector3.Distance(checkPoint.position, light.transform.position);

                if (distance > light.range) continue;

                Vector3 dirFromLightToPlayer = checkPoint.position - light.transform.position;

                if (light.type == LightType.Spot)
                {
                    float angleToPlayer = Vector3.Angle(light.transform.forward, dirFromLightToPlayer);

                    if (angleToPlayer > light.spotAngle / 2f)
                    {
                        continue;
                    }
                }
                if (!Physics.Raycast(checkPoint.position, -dirFromLightToPlayer, distance, obstacleLayer))
                {
                    float lightStrength = 1f - (distance / light.range);
                    totalIllumination += lightStrength * light.intensity;
                }
            }
            else if (light.type == LightType.Directional)
            {
                Vector3 dirToSun = -light.transform.forward;

                if (!Physics.Raycast(checkPoint.position, dirToSun, 100f, obstacleLayer))
                {
                    totalIllumination += light.intensity;
                }
            }
        }
    
        if (playerStats != null)
        {
            playerStats.illuminationLevel = Mathf.Clamp(totalIllumination, 0f, 3f);
        }
    }
}