using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{

    private Toggle _toggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _toggle = GetComponent<Toggle>();   
    }

    public void OnToggleClick()
    {
        Screen.fullScreen = _toggle.isOn;
    }
}
