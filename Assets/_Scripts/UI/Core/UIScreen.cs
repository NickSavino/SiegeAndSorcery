using System.Collections;
using UnityEngine;
namespace _Scripts.UI.Core
{
    public abstract class UIScreen : MonoBehaviour
    {
        public bool IsVisible => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
