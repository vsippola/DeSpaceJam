using UnityEngine;
using System.Collections;

public class ControllerMusicVolume : MonoBehaviour
{
    private SingletonJsonLoadable<SettingsGameDataPairs> settings;
    private AudioSource music;

    private float baseVol;   

    private void Start()
    {
        settings = SingletonJsonLoadable<SettingsGameDataPairs>.Instance;
        music = transform.GetComponentInChildren<AudioSource>();

        baseVol = music.volume;

        SetVolume();
        StartMusic();
    }

    public void UpdateVolume()
    {
        SetVolume();
        StartMusic();
    }

    private void SetVolume()
    {
        music.volume = baseVol * (settings.data.volume / 100f);
    }

    private void StartMusic()
    {
        bool playing = music.isPlaying;
        bool mute = settings.data.mute;

        if (playing && mute) music.Stop();
        else if (!playing && !mute) music.Play();
    }
}
