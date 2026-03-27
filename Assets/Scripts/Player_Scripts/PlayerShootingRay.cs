using System.Collections;
using UnityEngine;

public class PlayerShootingRay : MonoBehaviour
{
    public Transform muzzle;
    public Camera _camera;
    public LineRenderer _lineRenderer;

    private void ShootRay() 
    {
        RaycastHit hit;

        Vector3 shotOrigin = _camera.transform.position;
        Vector3 shotDirection = _camera.transform.forward;

        Ray _ray = _camera.ViewportPointToRay(new Vector3(0.3f, 0.3f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(shotOrigin, shotDirection, out hit, Mathf.Infinity))
        {
            targetPoint = hit.point;
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            Debug.Log("Hit object: " + hit.transform.name);
            _lineRenderer.SetPosition(1, hit.point);
            if (damageable != null)
            {
                damageable.TakeDamage(33.34f);
            }
        }
        else
        {
            targetPoint = _ray.GetPoint(150f);
        }

        StartCoroutine(RenderTracer(targetPoint));
    }

    IEnumerator RenderTracer(Vector3 target)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, muzzle.transform.position);
        _lineRenderer.SetPosition(1, target);

        yield return new WaitForSeconds(0.05f);
        _lineRenderer.enabled = false;
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
