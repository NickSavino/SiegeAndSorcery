using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingController : MonoBehaviour
{
    public static SceneLoadingController instance;

    public GameObject loadingScreen;
    public Image loadingBarFill;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        operation.allowSceneActivation = true;

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            displayedProgress = Mathf.MoveTowards(displayedProgress, progressValue, Time.unscaledDeltaTime / 2f);

            loadingBarFill.fillAmount = displayedProgress;

            if (loadingBarFill.fillAmount >= 0.9f)
            {
                loadingBarFill.fillAmount = 1f;

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
