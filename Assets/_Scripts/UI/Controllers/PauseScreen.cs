using System;
using _Scripts.UI.Core;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI.Controllers
{
    public class PauseScreen : UIScreen
    {

        [SerializeField]
        Button resumeButton;
        [SerializeField]
        Button settingsButton;
        [SerializeField]
        Button mainMenuButton;
        [SerializeField]
        Button quitButton;

        public event Action ResumeRequested;
        public event Action SettingsRequested;
        public event Action MainMenuRequested;
        public event Action QuitRequested;

        void OnEnable()
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        void OnDisable()
        {
            resumeButton.onClick.RemoveListener(OnResumeClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            mainMenuButton.onClick.RemoveListener(OnQuitClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        void OnResumeClicked()
        {
            ResumeRequested?.Invoke();
        }

        void OnSettingsClicked()
        {
            SettingsRequested?.Invoke();
        }

        void OnMainMenuClicked()
        {
            MainMenuRequested?.Invoke();
        }

        void OnQuitClicked()
        {
            QuitRequested?.Invoke();
        }
    }
}
