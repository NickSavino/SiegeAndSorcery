using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Slider utility to control audio volume. Should be attached to GameObject with slider component
/// </summary>
public class AudioSlider : MonoBehaviour
{

    private const float MINIMUM_SLIDER_VALUE = -79.9999f;
    private const float MAXIMUM_SLIDER_VALUE = 10f;

    public UnityEvent<float> volumeChangeEvent;

    private Slider _slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = MAXIMUM_SLIDER_VALUE;
        _slider.minValue = MINIMUM_SLIDER_VALUE;

        if ( _slider == null )
        {
            throw new NullReferenceException();
        }

        if (volumeChangeEvent == null)
        {
            volumeChangeEvent = new UnityEvent<float>();
        }

        OnVolumeChange();
    }

    public void OnVolumeChange()
    {
        volumeChangeEvent.Invoke(_slider.value);
    }
}