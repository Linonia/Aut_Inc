using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] soundtracks;
    
    public Sound[] soundEffects;
    
    [HideInInspector]
    public float soundtrackVolume = 1f;
    
    [HideInInspector]
    public float soundEffectVolume = 1f;
    
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        foreach (var sound in soundtracks)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        
        foreach (var sound in soundEffects)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void ChangeSoundtrackVolume(float volume)
    {
        foreach (var sound in soundtracks)
        {
            sound.volume = volume;
            sound.source.volume = volume;
        }
        soundtrackVolume = volume;
    }
    
    public void ChangeSoundEffectVolume(float volume)
    {
        foreach (var sound in soundEffects)
        {
            sound.volume = volume;
            sound.source.volume = volume;
        }
        soundEffectVolume = volume;
    }

    public void StartSoundtrack()
    {
        foreach (var sound in soundtracks)
        {
            sound.source.Play();
        }
    }
    
    public void StopSoundtrack()
    {
        foreach (var sound in soundtracks)
        {
            sound.source.Stop();
        }
    }
    
    public void PlaySoundTrack(string name)
    {
        Sound sound = Array.Find(soundtracks, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Soundtrack: " + name + " not found!");
            return;
        }
        sound.source.Play();
    }

    public void StopSoundTrack(string name)
    {
        Sound sound = Array.Find(soundtracks, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Soundtrack: " + name + " not found!");
            return;
        }
        sound.source.Stop();
    }

    public void StartSoundEffect()
    {
        foreach (var sound in soundEffects)
        {
            sound.source.Play();
        }
    }
    
    public void StopSoundEffect()
    {
        foreach (var sound in soundEffects)
        {
            sound.source.Stop();
        }
    }
    
    public void PlaySoundEffect(string name)
    {
        Sound sound = Array.Find(soundEffects, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound effect: " + name + " not found!");
            return;
        }
        sound.source.Play();
    }
    
    public void StopSoundEffect(string name)
    {
        Sound sound = Array.Find(soundEffects, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound effect: " + name + " not found!");
            return;
        }
        sound.source.Stop();
    }

    void Start()
    {
        StartSoundtrack();
    }
    
}
