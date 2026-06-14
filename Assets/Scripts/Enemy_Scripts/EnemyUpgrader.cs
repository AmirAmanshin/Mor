using UnityEngine;

public class EnemyUpgrader : MonoBehaviour
{
    public static EnemyUpgrader Instance { get; private set; }

    [SerializeField] private PlayerUI _playerUI;

    // Свойства. Unity их не видит, Инспектор их не сломает.
    public float CurrentEnemySpeed { get; private set; } = 3.5f;
    public float CurrentEnemyAttackCooldown { get; private set; } = 1.0f;

    private float upgradeCoefficent = 1.5f;
    private int lastUpgradedKillCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_playerUI == null)
        {
            GameObject playerContainer = GameObject.FindWithTag("Player");
            if (playerContainer != null)
                _playerUI = playerContainer.GetComponent<PlayerUI>();
        }
    }

    private void Update()
    {
        //if (_playerUI == null) return;

        //if (_playerUI.killAmount > 0 &&
        //    _playerUI.killAmount % 10 == 0 &&
        //    _playerUI.killAmount != lastUpgradedKillCount)
        //{
        //    Upgrade();
        //    lastUpgradedKillCount = _playerUI.killAmount;
        //}
    }

    private void Upgrade()
    {
        CurrentEnemyAttackCooldown /= upgradeCoefficent;
        CurrentEnemySpeed *= upgradeCoefficent;
        Debug.Log($"Stats upgraded! New Speed: {CurrentEnemySpeed}, Cooldown: {CurrentEnemyAttackCooldown}");
    }

    public void ResetUpgrades()
    {
        CurrentEnemySpeed = 3.5f;
        CurrentEnemyAttackCooldown = 1.0f;
        lastUpgradedKillCount = 0;
        Debug.Log("Enemy upgrades reset to default.");
    }
}