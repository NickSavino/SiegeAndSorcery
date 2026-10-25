using _Scripts.Constants;
using _Scripts.Controllers;
using _Scripts.UI.Core;
using UnityEngine;
namespace _Scripts.UI.Controllers
{
    public class GameplayMenuController : MonoBehaviour
    {
        [SerializeField]
        ScreenNavigator screenNavigator;

        [SerializeField]
        PauseScreen pauseScreen;

        [SerializeField]
        OptionsScreen optionsScreen;

        float _previousTimeScale = 1f;
        
        public bool IsPaused { get; private set; }

        bool _isReturningToMenu;

        void OnEnable()
        {
            pauseScreen.ResumeRequested += Resume;
            pauseScreen.SettingsRequested +=  OpenOptions;
            pauseScreen.MainMenuRequested += ReturnToMainMenu;
            pauseScreen.QuitRequested += Quit;

            optionsScreen.BackButtonRequested += GoBack;
        }

        public void OnDisable()
        {
            {
                if (pauseScreen != null)
                {
                    pauseScreen.ResumeRequested -= Resume;
                    pauseScreen.SettingsRequested -= OpenOptions;
                    pauseScreen.QuitRequested -= Quit;
                }

                if (optionsScreen != null)
                {
                    
                    optionsScreen.BackButtonRequested -= GoBack;
                }
            }
            
            Resume();
        }

        void Start()
        {
            screenNavigator.Initialize();
        }

        void Update()
        {
            if (_isReturningToMenu || !Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            if (!IsPaused)
            {
                Pause();
            }
            else if (screenNavigator.CanGoBack)
            {
                GoBack();
            }
            else
            {
                Resume();
            }
        }

        public void Pause()
        {
            if (IsPaused || _isReturningToMenu)
            {
                return;
            }
            
            _previousTimeScale = Time.timeScale;
            IsPaused = true;

            Time.timeScale = 0f;
            screenNavigator.Open(pauseScreen);
        }

        public void Resume()
        {
            if (!IsPaused)
            {
                return;
            }

            IsPaused = false;
            Time.timeScale = _previousTimeScale;

            if (screenNavigator != null)
            {
                screenNavigator.CloseAll();
            }
        }

        void OpenOptions()
        {
            screenNavigator.Open(optionsScreen);
        }

        void ReturnToMainMenu()
        {
            if (_isReturningToMenu)
            {
                return;
            }

            var loader = SceneLoadingController.Instance;

            if (loader == null)
            {
                Debug.LogError("Scene Loader Unavailable", this);
                return;
            }

            _isReturningToMenu = true;
            
            Resume();

            Time.timeScale = 1f;
            loader.LoadScene(SceneNames.MainMenuScene);
        }

        void GoBack()
        {
            screenNavigator.Back();
        }

        static void Quit()
        {
            Application.Quit();
        }
    }
}
