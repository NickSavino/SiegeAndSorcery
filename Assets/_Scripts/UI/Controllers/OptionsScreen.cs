using System;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI.Controllers
{
    public class OptionsScreen : MonoBehaviour
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
