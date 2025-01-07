using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class MusicSystem : MonoBehaviour
{
    private bool _titleMusicPlaying = false;

    AudioSource titleMusicSource;

    private float _musicVolume;

    AudioSource _currentSource;

    public AudioMixer mixer;

    [SerializeField]
    public AudioMixerGroup group;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentSource = titleMusicSource;
        titleMusicSource.PlayDelayed(2f);
        _titleMusicPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
    }

    public void ChangeMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

    }
}
