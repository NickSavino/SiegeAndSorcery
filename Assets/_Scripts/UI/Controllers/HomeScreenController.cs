using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenController : MonoBehaviour
{

    [SerializeField]
    Button playButton;
    [SerializeField]
    Button settingsButton;
    [SerializeField]
    Button quitButton;

    public event Action PlayRequested;
    public event Action SettingsRequested;
    public event Action QuitRequested;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    void OnPlayClicked()
    {
        PlayRequested?.Invoke();
    }

    void OnSettingsClicked()
    {
        SettingsRequested?.Invoke();
    }

    void OnQuitClicked()
    {
        QuitRequested?.Invoke();
    }
}
