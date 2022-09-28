using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Level's music")]
    [SerializeField] AudioSource theme = null;
    [Header("Settings")]
    [SerializeField] AudioSource soundEffectTest = null;
    [SerializeField] float musicVolume = 1f;
    [SerializeField] float soundEffectVolume = 1f;

    [Range(0f, 1f)] public float pauseVolumeScale = 0.5f;
    float volumeScale = 1f;

    GameMaster master = null;

    private void Start()
    {
        master = FindObjectOfType<GameMaster>();
        musicVolume = master.musicVolume;
        soundEffectVolume = master.soundEffectVolume;
        ChangeMusicVolume();
        ChangeSoundEffectVolume();
    }

    public void SetMusicVolume(float volume) 
    {
        musicVolume = master.musicVolume = volume * volumeScale;
        ChangeMusicVolume();
    }
    public void SetSoundEffectVolume(float volume) 
    {
        soundEffectVolume = master.soundEffectVolume = volume * volumeScale;
        ChangeSoundEffectVolume();
    }

    private void ChangeSoundEffectVolume()
    {
        foreach(AudioSource sound in FindObjectsOfType<AudioSource>())
        {
            if (!sound.CompareTag("Music"))
                sound.volume = soundEffectVolume;
        }

        if (soundEffectTest != null) soundEffectTest.volume = soundEffectVolume;
    }

    private void ChangeMusicVolume()
    {
        if (theme != null) 
            theme.volume = musicVolume;
    }

    public void PlaySoundEffect()
    {
        if (soundEffectTest != null) soundEffectTest.Play();
    }

    public void SetVolumeScale(float scale)
    {
        ResetVolumeScale();
        volumeScale = scale;
        soundEffectVolume *= volumeScale;
        musicVolume *= volumeScale;

        ChangeSoundEffectVolume();
        ChangeMusicVolume();
    }

    public void ResetVolumeScale()
    {
        if (soundEffectVolume != 0f) soundEffectVolume /= volumeScale;
        if (musicVolume != 0f) musicVolume /= volumeScale;

        volumeScale = 1f;

        ChangeSoundEffectVolume();
        ChangeMusicVolume();
    }

    public void OpenSoundSettingsInPause()
    {
        foreach (AudioSource sound in FindObjectsOfType<AudioSource>())
            if (!sound.CompareTag("Music") && sound.loop) sound.Stop();
    }

    public void CloseSoundSettingsInPause()
    {
        foreach (AudioSource sound in FindObjectsOfType<AudioSource>())
            if (!sound.CompareTag("Music") && sound.loop && sound.playOnAwake) sound.Play();
    }
}
