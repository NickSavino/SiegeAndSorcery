using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrapper class that initializes static data and singletons and startup
/// </summary>
public static class BootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Execute()
    {
        #if UNITY_EDITOR
        var currentlyLoadedEditorScene = SceneManager.GetActiveScene();
        #endif

        if (SceneManager.GetSceneByName("BootStrapperScene").isLoaded != true)
            SceneManager.LoadScene("BootStrapperScene", LoadSceneMode.Additive);

        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
    }
}