using System;
using System.Collections.Generic;
using _Scripts.UI.Core;
using UnityEngine;

namespace _Scripts.UI.Controllers
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        HomeScreen homeScreen;
        [SerializeField]
        OptionsScreen optionsScreen;
        [SerializeField]
        LevelSelectorScreen levelSelectorScreen;

        [SerializeField]
        ScreenNavigator screenNavigator;

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
            screenNavigator.Initialize(homeScreen);
        }

        void GoBack()
        {
            screenNavigator.Back();
        }

        void OpenLevelSelector()
        {
            screenNavigator.Open(levelSelectorScreen);
        }

        void OpenOptionsScreen()
        {
            screenNavigator.Open(optionsScreen);
        }

        static void Quit()
        {
            Application.Quit();
        }
    }
}
