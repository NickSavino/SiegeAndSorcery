using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundCycler : MonoBehaviour
{

    public List<AudioClip> audioSources;

    public AudioClip selectedClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSources == null)
        {
            audioSources = new List<AudioClip>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip SelectRandomSound()
    {
        var sourceToPlay = audioSources[Random.Range(0, audioSources.Count)];

        return sourceToPlay;
    }
}
