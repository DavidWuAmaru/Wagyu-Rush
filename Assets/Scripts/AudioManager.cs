using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound[] musics;
    [SerializeField] private Slider SESlider;
    [SerializeField] private Slider musicSlider;
    [Range (0.0f, 1.5f)] [SerializeField] private float SEAmplifier = 1.0f;
    [Range (0.0f, 1.5f)] [SerializeField] private float MusicAmplifier = 1.0f;
    public static AudioManager audioManager;
    private void Awake()
    {
        if (audioManager == null) audioManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume * SEAmplifier;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound m in musics)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;

            m.source.volume = m.volume * MusicAmplifier;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
        }

        PlayMusic("BGM");
    }
    
    public void updateSEVolume()
    {
        SEAmplifier = SESlider.value;
        Debug.Log(SEAmplifier);
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * SEAmplifier;
        }
    }
    public void updateMusicVolume()
    {
        MusicAmplifier = musicSlider.value;
        Debug.Log(MusicAmplifier);
        foreach (Sound m in musics)
        {
            m.source.volume = m.volume * MusicAmplifier;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
        s.source.Play();
    }
    public bool isSoundPlaying(string name)
    {
        foreach(Sound s in sounds)
        {
            if (s.name == name && s.source!= null && s.source.isPlaying) return true;
        }
        return false;
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void PlayMusic(string name)
    {
        Sound m = Array.Find(musics, music => music.name == name);
        if (m == null)
        {
            Debug.LogError("Music " + name + " not found!");
            return;
        }
        m.source.Play();
    }
    public bool isMusicPlaying(string name)
    {
        foreach (Sound m in musics)
        {
            if (m.name == name && m.source != null && m.source.isPlaying) return true;
        }
        return false;
    }
    public void StopMusic(string name)
    {
        Sound m = Array.Find(musics, music => music.name == name);
        if (m == null)
        {
            Debug.LogError("Music " + name + " not found!");
            return;
        }
        m.source.Stop();
    }
}
