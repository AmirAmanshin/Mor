using UnityEngine;

public class PlayerShootingRay : MonoBehaviour
{
    private EnemyStats _enemyStats;
    public Transform shootPoint;

    private void ShootRay()
    {
        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, Mathf.Infinity))
        {
            _enemyStats = GetComponent<EnemyStats>();

            Debug.Log("Hit object: " + hit.transform.name);
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit!\n-1/2 hp");
                _enemyStats.health -= 50;
            }
        }
    }


    void Start()
    {

    }

    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ShootRay();
        }
    }
}
