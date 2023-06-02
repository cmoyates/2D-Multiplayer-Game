using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSrc;
    public AudioMixerGroup musicGroup;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            musicGroup.audioMixer.SetFloat("LOWPASS_CUTOFF", 672);

        }
        else 
        {
            musicGroup.audioMixer.SetFloat("LOWPASS_CUTOFF", 22000);
        }
    }
}
