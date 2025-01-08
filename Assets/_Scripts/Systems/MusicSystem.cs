using UnityEngine;

public class MusicSystem : AudioSystemBase
{
    private bool _titleMusicPlaying = false;

    AudioSource titleMusicSource;

    public override void ChangeVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
    }

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
}
