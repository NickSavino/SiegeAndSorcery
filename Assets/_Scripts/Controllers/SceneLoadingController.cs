using System.Collections;
using _Scripts.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Controllers
{
    /// <summary>
    /// Any scene management should be controlled through this class
    /// </summary>
    public class SceneLoadingController : MonoBehaviour
    {
        private enum StartupDestination
        {
            KeepCurrentScene,
            MainMenu,
            LevelSelector
        }

        [SerializeField] private StartupDestination startupDestination = StartupDestination.MainMenu;

        public static SceneLoadingController Instance;

        public GameObject loadingScreen;
        public Image loadingBarFill;

        public GameObject fade;

        public Animator animator;

        [SerializeField] public bool loadMainMenu;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            loadingScreen.SetActive(false);
            fade.SetActive(false);

            if (!SceneManager.GetSceneByName(SceneNames.BootStrapperScene).isLoaded) return;

            SceneManager.UnloadSceneAsync(SceneNames.BootStrapperScene);
            switch (startupDestination)
            {
                case StartupDestination.KeepCurrentScene:
                    break;
                case StartupDestination.MainMenu:
                    if (!SceneManager.GetSceneByName(SceneNames.MainMenuScene).isLoaded)
                        LoadScene(SceneNames.MainMenuScene);

                    break;
                case StartupDestination.LevelSelector:
                    if (!SceneManager.GetSceneByName(SceneNames.LevelSelectorScene).isLoaded)
                        LoadScene(SceneNames.LevelSelectorScene);
                    break;
                default:
                    LoadScene(SceneNames.MainMenuScene);
                    break;
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(HandleSceneTransition(sceneName));
        }

        /// <summary>
        /// Handles fading in and out when loading new scene
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator HandleSceneTransition(string sceneName)
        {
            // Close scene
            fade.SetActive(true);
            FadeOut();
            yield return new WaitForSeconds(GetAnimationDuration("FadeOut"));

            // Transition to loading screen
            loadingScreen.SetActive(true);
            FadeIn();
            yield return new WaitForSeconds(GetAnimationDuration("FadeIn"));

            // Run scene loader
            yield return StartCoroutine(LoadSceneAsync(sceneName));

            // Fade out loading screen
            FadeOut();
            yield return new WaitForSeconds(GetAnimationDuration("FadeOut"));

            loadingScreen.SetActive(false);

            // Fade in new scene
            FadeIn();
            yield return new WaitForSeconds(GetAnimationDuration("FadeIn"));

            fade.SetActive(false);

            yield return null;
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);

            if (operation == null) yield return null;

            operation!.allowSceneActivation = false;
            var displayedProgress = 0f;

            while (!operation.isDone)
            {
                var progressValue = Mathf.Clamp01(operation.progress / 0.9f);

                displayedProgress = Mathf.MoveTowards(displayedProgress, progressValue, Time.unscaledDeltaTime / 3f);

                loadingBarFill.fillAmount = displayedProgress;

                if (loadingBarFill.fillAmount >= 0.9f)
                {
                    loadingBarFill.fillAmount = 1f;

                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        private void FadeOut()
        {
            animator.ResetTrigger("FadeIn");
            animator.SetTrigger("FadeOut");
        }

        private void FadeIn()
        {
            animator.ResetTrigger("FadeOut");
            animator.SetTrigger("FadeIn");
        }

        private float GetAnimationDuration(string animationName, float additionalTime = 0f)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
                if (clip.name == animationName)
                    return clip.length + additionalTime;

            Debug.LogWarning($"Animation '{animationName}' not found!");
            return 0.5f; // Default fallback duration
        }
    }
}