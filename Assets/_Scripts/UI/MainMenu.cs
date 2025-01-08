using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMenu : MonoBehaviour
{
    public GameObject optionsScreen;
    public GameObject menuScreen;

    private GameObject _currentScreen;

    private Stack<GameObject> _navigationStack;

    public void Start()
    {
        _navigationStack = new Stack<GameObject>();
        optionsScreen.SetActive(false);
        menuScreen.SetActive(true);
        _currentScreen = menuScreen;
    }

    public void OnPlayButtonPress()
    {
        SceneManager.LoadScene("AudioScene");
    }

    public void OnQuitButtonPress()
    {
        Application.Quit();
    }

    public void OnOptionsButtonPress()
    {
        SetCurrentScreen(optionsScreen);
    }

    public void OnBackButtonPress()
    {
        ReturnToPreviousScreen();
    }

    // helper that updates current scene and adds previous to stack
    private void SetCurrentScreen(GameObject screen)
    {
        _currentScreen.SetActive(false);
        _navigationStack.Push(_currentScreen);
        screen.SetActive(true);
        _currentScreen = screen;
    }

    private void ReturnToPreviousScreen()
    {
        _currentScreen.SetActive(false);
        var prevScreen = _navigationStack.Pop();
        prevScreen.SetActive(true);
        _currentScreen = prevScreen;
    }
}
