using UnityEngine;

public class MusicSystem : AudioSystemBase
{
    AudioSource musicSource;

    public override void ChangeVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentSource = musicSource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
