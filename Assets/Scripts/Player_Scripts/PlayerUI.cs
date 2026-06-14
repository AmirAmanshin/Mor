using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerStealthState _stealthState;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider noiseLevelSlider;
    [SerializeField] private Slider visibilitySlider;

    private void Awake()
    {
        healthSlider.maxValue = 100;
        noiseLevelSlider.maxValue = 100;
        visibilitySlider.maxValue = 100;
    }

    void Start()
    {
        healthSlider.value = _playerStats.health;
        noiseLevelSlider.value = _playerStats.noiseLevel;
        visibilitySlider.value = _playerStats.visibility;
    }

    void Update()
    {
        healthSlider.value = _playerStats.health;
        noiseLevelSlider.value = _playerStats.noiseLevel;
        visibilitySlider.value = _playerStats.visibility;
    }
}
