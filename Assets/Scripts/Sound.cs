using UnityEngine;

// Script inspired by this video : https://www.youtube.com/watch?v=6OT43pvUyfY&t=710s

[System.Serializable] 
public class Sound
{
    public string name;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;

    public AudioClip clip = null;
    [HideInInspector] public AudioSource source = null;

    public void Play()
    {
        if (source != null) source.Play();
    }
}
