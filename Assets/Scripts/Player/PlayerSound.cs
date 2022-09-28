using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerSoundUnite
{
    public string key;
    public AudioSource sound;
}

public class PlayerSound : MonoBehaviour
{
    [SerializeField] List<PlayerSoundUnite> sounds = new List<PlayerSoundUnite>();
    
    public void Play(string key)
    {
        AudioSource temp = Find(key);
        if (temp != null) temp.Play();
    }

    public void PlayOnce(string key)
    {
        AudioSource temp = Find(key);
        if (temp != null && !temp.isPlaying) temp.Play();
    }

    public void Stop(string key)
    {
        AudioSource temp = Find(key);
        if (temp != null) temp.Stop();
    }

    private AudioSource Find(string key)
    {
        return sounds.Find(sound => key.Equals(sound.key)).sound;
    }
}
