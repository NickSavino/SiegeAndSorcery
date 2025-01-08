using UnityEngine;
using UnityEngine.SceneManagement;

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