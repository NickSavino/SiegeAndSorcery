using UnityEngine;

public class MusicSystem : AudioSystemBase
{
    public AudioSource musicSource;

    public static MusicSystem instance;

    public override void ChangeVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
    }


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
        _currentSource = musicSource;

        PlayCurrentSource();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
