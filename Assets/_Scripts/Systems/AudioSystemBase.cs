using UnityEngine;
using UnityEngine.Audio;

public abstract class AudioSystemBase : MonoBehaviour
{

    protected AudioSource _currentSource;

    public AudioMixer mixer;

    [SerializeField]
    public AudioMixerGroup group;

    public abstract void ChangeVolume(float volume);

    public virtual void SetAudioSource(AudioSource source)
    {
        _currentSource = source;
    }

    public virtual void PlaySource(AudioSource source)
    {
        _currentSource = source;
        source.Play();
    }

    public virtual void PlayCurrentSource()
    {
        if (_currentSource != null)
        {
            _currentSource.Play();
        }
    }

    public virtual void StopCurrentSource()
    {

        if (_currentSource != null)
        {
            _currentSource.Stop();
        }
    }

    public virtual void PauseCurrentSource()
    {
        if (_currentSource != null)
        {
            _currentSource.Pause();
        }
    }
}
