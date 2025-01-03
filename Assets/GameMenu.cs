using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas pauseMenu;

    public void Start()
    {
        pauseMenu.enabled = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.enabled = !pauseMenu.enabled;
            Time.timeScale = Time.timeScale == 0 ? Time.timeScale = 1 : Time.timeScale = 0;
        }
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnOptionsButtonClick()
    {

    }

    public void OnResumeButtonClick()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1;
    }

}
