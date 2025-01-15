using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundSystem : AudioSystemBase
{

    public static SoundSystem instance;

    [System.Serializable]
    public class SoundEffects
    {
        public string name;
        public AudioSource audioSource;
    }

    public List<SoundEffects> soundEfects;

    private static Dictionary<string, AudioSource> _soundEffectsDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (soundEfects == null)
        {
            soundEfects = new List<SoundEffects>();
        }

        _soundEffectsDictionary = new Dictionary<string, AudioSource>();
        _soundEffectsDictionary = soundEfects.ToDictionary(x => x.name, x => x.audioSource);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string name)
    {
        AudioSource sound;

        _soundEffectsDictionary.TryGetValue(name, out sound);

        SetAudioSource(sound);
        PlayCurrentSource();
    }

    public void PlaySound(AudioSource sound)
    {
        if (sound != null)
        {
            PlaySource(sound);
        }
    }

    public void OnBackButtonClick()
    {
        AudioSource sound;

        _soundEffectsDictionary.TryGetValue("BackButtonClick", out sound);

        SetAudioSource(sound);
        PlayCurrentSource();
    }

    public override void ChangeVolume(float volume)
    {
        mixer.SetFloat("SoundVolume", volume);
    }
}
