using System.Collections.Generic;
using UnityEngine;
namespace _Scripts.UI.Core
{
    public class ScreenNavigator : MonoBehaviour
    {
        [SerializeField]
        RectTransform screenContainer;
        
        readonly Stack<UIScreen> _history = new Stack<UIScreen>();
        
        UIScreen _currentScreen;
        
        public bool CanGoBack => _history.Count > 0;

        public bool HasOpenScreen => _currentScreen != null;

        public void Initialize(UIScreen initialScreen = null)
        {
            if (screenContainer == null)
            {
                Debug.LogError("ScreenNavigator requires a container.", this);
                return;
            }
            
            CloseAll();

            foreach (var screen in screenContainer.GetComponentsInChildren<UIScreen>(true))
            {
                screen.Hide();
            }

            if (initialScreen != null)
            {
                Open(initialScreen);
            }
        }

        public void Open(UIScreen nextScreen)
        {
            if (nextScreen == null || nextScreen == _currentScreen)
            {
                return;
            }

            if (_currentScreen != null)
            {
                _history.Push(_currentScreen);
                _currentScreen.Hide();
            }
            
            _currentScreen = nextScreen;
            _currentScreen.Show();
        }

        public void Back()
        {
            if (!CanGoBack)
            {
                return;
            }
            
            _currentScreen.Hide();
            _currentScreen = _history.Pop();
            _currentScreen.Show();
        }

        public void CloseAll()
        {
            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }

            _currentScreen = null;
            _history.Clear();
        }
    }
}
