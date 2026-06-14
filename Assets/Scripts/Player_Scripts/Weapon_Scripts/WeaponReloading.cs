using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponReloading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private float reloadTime = 1.5f;

    private int currentAmmo;

    public bool IsReloading { get; private set; }

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateUI();
    }

    public bool HasAmmo() => currentAmmo > 0;

    public void ConsumeAmmo()
    {
        currentAmmo--;
        UpdateUI();
    }

    public void StartReload(WeaponVisuals visuals)
    {
        if (!IsReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine(visuals));
        }
    }

    private IEnumerator ReloadRoutine(WeaponVisuals visuals)
    {
        IsReloading = true;
        visuals.SetWeaponVisibility(false);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        UpdateUI();

        visuals.SetWeaponVisibility(true);
        IsReloading = false;
    }

    private void UpdateUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / ∞";
        }
    }
}