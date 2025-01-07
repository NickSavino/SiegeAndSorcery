using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Slider utility to control audio volume. Should be attached to GameObject with slider component
/// </summary>
public class AudioSlider : MonoBehaviour
{

    public UnityAudioEvent volumeChangeEvent;

    private Slider _slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slider = GetComponent<Slider>();

        if ( _slider == null )
        {
            throw new NullReferenceException();
        }

        if (volumeChangeEvent == null)
        {
            volumeChangeEvent = new UnityAudioEvent();
        }

        OnVolumeChange();
    }

    public void OnVolumeChange()
    {
        volumeChangeEvent.Invoke(_slider.value);
    }
}

[System.Serializable]
public class UnityAudioEvent : UnityEvent<float> { }