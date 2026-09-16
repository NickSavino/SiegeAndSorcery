using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.UI.Controllers
{
    public class MenuController : MonoBehaviour
    {
        readonly Stack<GameObject> _history = new Stack<GameObject>();

        [SerializeField]
        HomeScreen homeScreen;
        [SerializeField]
        OptionsScreen optionsScreen;
        [SerializeField]
        LevelSelectorScreen levelSelectorScreen;

        GameObject _currentScreen;

        void OnEnable()
        {
            homeScreen.PlayRequested += OpenLevelSelector;
            homeScreen.SettingsRequested += OpenOptionsScreen;
            homeScreen.QuitRequested += Quit;

            optionsScreen.BackButtonRequested += GoBack;

            levelSelectorScreen.BackButtonRequested += GoBack;
        }

        void OnDisable()
        {
            homeScreen.PlayRequested -= OpenLevelSelector;
            homeScreen.SettingsRequested -= OpenOptionsScreen;
            homeScreen.QuitRequested -= Quit;

            optionsScreen.BackButtonRequested -= GoBack;

            levelSelectorScreen.BackButtonRequested -= GoBack;
        }

        void Start()
        {
            optionsScreen.gameObject.SetActive(false);
            levelSelectorScreen.gameObject.SetActive(false);

            _currentScreen = homeScreen.gameObject;
            _currentScreen.SetActive(true);
        }

        void GoBack()
        {
            if (_history.Count == 0)
            {
                return;
            }

            _currentScreen.SetActive(false);
            _currentScreen = _history.Pop();
            _currentScreen.SetActive(true);
        }

        void OpenLevelSelector()
        {
            OpenScreen(levelSelectorScreen.gameObject);
        }

        void OpenOptionsScreen()
        {
            OpenScreen(optionsScreen.gameObject);
        }

        void OpenScreen(GameObject nextScreen)
        {
            if (nextScreen == null)
            {
                return;
            }

            _history.Push(_currentScreen);
            _currentScreen.SetActive(false);

            _currentScreen = nextScreen;
            _currentScreen.SetActive(true);

        }

        static void Quit()
        {
            Application.Quit();
        }
    }
}
