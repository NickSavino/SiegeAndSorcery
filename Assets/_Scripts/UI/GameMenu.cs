using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas pauseMenu;
    public Canvas pauseMenuOptions;

    private Canvas _currentScreen;

    private Stack<Canvas> _navigationStack;

    public void Start()
    {
        _navigationStack = new Stack<Canvas>();
        pauseMenu.enabled = false;
        pauseMenuOptions.enabled = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.enabled = !pauseMenu.enabled;
            _currentScreen = pauseMenu.enabled ? pauseMenu : null;
            Time.timeScale = Time.timeScale == 0 ? Time.timeScale = 1 : Time.timeScale = 0;
        }
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnOptionsButtonClick()
    {
        pauseMenuOptions.enabled = true;
        _currentScreen.enabled = false;
        _navigationStack.Push(_currentScreen);
        _currentScreen = pauseMenuOptions;
    }

    public void OnBackButtonClick()
    {
        _currentScreen.enabled = false;
        var prevScreen = _navigationStack.Pop();
        prevScreen.enabled = true;
        _currentScreen = prevScreen;
    }

    public void OnResumeButtonClick()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1;
    }

}
