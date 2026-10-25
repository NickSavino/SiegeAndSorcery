using System;
using _Scripts.UI.Core;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI.Controllers
{
    public class OptionsScreen : UIScreen
    {

        [SerializeField]
        Button backButton;

        public event Action BackButtonRequested;

        public void OnEnable()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void OnDisable()
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        void OnBackButtonClicked()
        {
            BackButtonRequested?.Invoke();
        }
    }
}
