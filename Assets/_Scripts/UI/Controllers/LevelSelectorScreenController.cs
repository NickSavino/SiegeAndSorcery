using System;
using System.Collections.Generic;
using _Scripts.Controllers;
using _Scripts.UI.Components;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI.Controllers
{
    public class LevelSelectorScreenController : MonoBehaviour
    {

        [Serializable]
        class LevelEntry
        {
            public string displayName;
            public string sceneName;
        }

        [SerializeField]
        MenuButton buttonPrefab;
        [SerializeField]
        Transform buttonContainer;

        [SerializeField]
        VerticalLayoutGroup verticalLayoutGroup;

        bool _isLoading;

        [Header("Levels")][SerializeField]
        List<LevelEntry> levels = new List<LevelEntry>();


        [SerializeField]
        Button backButton;

        public event Action BackButtonRequested;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Start()
        {
            BuildLevelList();
        }

        public void OnEnable()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void OnDisable()
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        public void OnBackButtonClicked()
        {
            BackButtonRequested?.Invoke();
        }

        void BuildLevelList()
        {
            foreach (var levelEntry in levels)
            {
                var menuButton = Instantiate(buttonPrefab, buttonContainer);

                menuButton.Initialize(
                    levelEntry.displayName,
                    () => LoadLevel(levelEntry.sceneName)
                );

            }
        }

        void LoadLevel(string sceneName)
        {
            if (_isLoading)
            {
                return;
            }

            if (SceneLoadingController.Instance == null)
            {
                Debug.LogError("No scene loader is available.", this);
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene `{sceneName} is unavailable. Check build scene list.", this);
                return;
            }

            _isLoading = true;

            SceneLoadingController.Instance.LoadScene(sceneName);
        }
    }
}
