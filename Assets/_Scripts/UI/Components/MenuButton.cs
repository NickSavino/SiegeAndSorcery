using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.UI.Components
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField]
        Button button;
        [SerializeField]
        TMP_Text label;

        UnityAction _clickAction;

        public void Initialize(string text, UnityAction onClick)
        {
            label.text = text;
            label.alignment = TextAlignmentOptions.Center;

            if (_clickAction != null)
            {
                button.onClick.RemoveListener(_clickAction);
            }

            _clickAction = onClick;

            if (_clickAction != null)
            {
                button.onClick.AddListener(_clickAction);
            }
        }

        public void AddClickListener(UnityAction clickAction)
        {
            _clickAction += clickAction;
            button.onClick.AddListener(_clickAction);
        }

        public void SetText(string text)
        {
            label.text = text;
        }

    }
}
