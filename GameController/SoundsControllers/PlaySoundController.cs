using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundController : MonoBehaviour
{
    public bool onAwake;

    private AudioSource sound;
    private float startSound;

    private SingletonJsonLoadable<SettingsGameDataPairs> settings;

    private void OnEnable()
    {
        settings = SingletonJsonLoadable<SettingsGameDataPairs>.Instance;
        sound = GetComponentInChildren<AudioSource>();
        startSound = sound.volume;
        if (!onAwake) return;
        PlaySound();
    }

    public void PlaySound()
    {
  
        if (settings.data.mute) return;
        sound.volume = startSound * settings.data.volume / 100f;
        sound.Play();
    }



}
