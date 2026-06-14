using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip emptyClip;

    [Header("Settings")]
    [Range(0, 1)][SerializeField] private float volume = 0.5f;
    [SerializeField] private float pitchRandomness = 0.05f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        _audioSource.spatialBlend = 0f;
    }

    public void PlayShoot() => PlaySound(shootClip, true);
    public void PlayReload() => PlaySound(reloadClip, false);
    public void PlayEmpty() => PlaySound(emptyClip, false);

    private void PlaySound(AudioClip clip, bool randomizePitch)
    {
        if (clip == null) return;

        _audioSource.pitch = randomizePitch ? 1f + Random.Range(-pitchRandomness, pitchRandomness) : 1f;
        _audioSource.PlayOneShot(clip, volume);
    }
}