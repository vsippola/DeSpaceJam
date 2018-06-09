using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    public delegate void OnSettingsChangeEvent();
    public event OnSettingsChangeEvent SettingsChangeEvent;

    public Button exit;

    public Toggle mute;
    public Slider volume;
    public Slider rotation;

    private SingletonJsonLoadable<SettingsGameDataPairs> settings;
    private ControllerMusicVolume contVolume;

    private void Start()
    {
        SetupVolume();

        mute.onValueChanged.AddListener(OnMuteChange);
        volume.onValueChanged.AddListener(OnVolumeChange);
        rotation.onValueChanged.AddListener(OnRotationChange);
    }
    private void OnEnable()
    {
        if (settings == null) SetupData();
        mute.isOn = settings.data.mute;
        volume.value = (settings.data.volume / 100f);
        rotation.value = (settings.data.rotateSpeed / 100f);
    }

    private void SetupData()
    {
        settings = SingletonJsonLoadable<SettingsGameDataPairs>.Instance;
        exit.onClick.AddListener(() => settings.Save());
    }

    private void SetupVolume()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GlobalMusic");
        contVolume = obj.GetComponentInChildren<ControllerMusicVolume>();
        SettingsChangeEvent += contVolume.UpdateVolume;
    }

    private void SignalChange()
    {
        if (SettingsChangeEvent != null) SettingsChangeEvent();
    }

    private void OnMuteChange(bool value)
    {
        if (settings == null) SetupData();
        if (contVolume == null) SetupVolume();

        settings.data.mute = value;
        SignalChange();
    }

    private void OnVolumeChange(float value)
    {
        if (settings == null) SetupData();
        if (contVolume == null) SetupVolume();

        settings.data.volume = value * 100;
        SignalChange();
    }

    private void OnRotationChange(float value)
    {
        if (settings == null) SetupData();
        settings.data.rotateSpeed = value * 100;
    }

    private void OnDestroy()
    {
        exit.onClick.RemoveAllListeners();
    }


}

