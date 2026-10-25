using System;
using _Scripts.UI.Core;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI.Controllers
{
    public class HomeScreen : UIScreen
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

        void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        void OnDisable()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
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
}
