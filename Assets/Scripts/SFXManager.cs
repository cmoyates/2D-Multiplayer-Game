using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public AudioMixerGroup sfxMixer;
    public AudioClip shootClip;
    public AudioClip hurtClip;
    public AudioClip dashClip;
    public AudioClip pickupCoinClip;
    public AudioClip pickupUpgradeClip;
    public AudioClip pickupHealthClip;
    public AudioClip teleportClip;

    private void Awake()
    {
        Instance = this;
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f, AudioMixerGroup group = null)
    {
        if (clip == null) return;
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        if (group != null)
            audioSource.outputAudioMixerGroup = group;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }

    public void PlayShootSFX(Vector3 pos) 
    {
        PlayClipAtPoint(shootClip, pos, 1, sfxMixer);
    }

    public void PlayHurtSFX(Vector3 pos) 
    {
        PlayClipAtPoint(hurtClip, pos, 1, sfxMixer);
    }

    public void PlayDashSFX(Vector3 pos, float volume = 2.0f) 
    {
        PlayClipAtPoint(dashClip, pos, volume, sfxMixer);
    }

    public void PlayPickupCoinClip(Vector3 pos, float volume = 1.0f) 
    {
        PlayClipAtPoint(pickupCoinClip, pos, volume, sfxMixer);
    }

    public void PlayPickupHealthClip(Vector3 pos, float volume = 1.0f)
    {
        PlayClipAtPoint(pickupHealthClip, pos, volume, sfxMixer);
    }
    public void PlayPickupUpgradeClip(Vector3 pos, float volume = 1.0f)
    {
        PlayClipAtPoint(pickupUpgradeClip, pos, volume, sfxMixer);
    }

    public void PlayTeleportClip(Vector3 pos, float volume = 1.0f)
    {
        PlayClipAtPoint(pickupUpgradeClip, pos, volume, sfxMixer);
    }
}
