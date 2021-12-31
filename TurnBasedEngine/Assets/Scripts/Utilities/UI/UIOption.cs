using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace BF2D.UI {
    public struct UIOptionData
    {
        public string text;
        public Sprite icon;
        public Sprite cursor;
        public Action action;
    };

    public class UIOption : MonoBehaviour {
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI _textMesh = null;
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image _image = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image _cursor = null;
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent _event = new UnityEvent();

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public bool Setup(UIOptionData optionData) {
            if (optionData.text != null) {
                _textMesh.text = optionData.text;
            } else {
                if (_textMesh != null)
                {
                    _textMesh.gameObject.SetActive(false);
                }
            }

            if (optionData.icon != null) {
                _image.sprite = optionData.icon;
            } else {
                if (_image != null)
                {
                    _image.gameObject.SetActive(false);
                }
            }

            if (optionData.cursor == null)
            {
                Debug.Log("[UIOption]: No sprite for cursor was given");
                return false;
            }

            _cursor.sprite = optionData.cursor;

            if (optionData.action != null)
            {
                _event.AddListener(() =>
                {
                    optionData.action();
                });
            }

            return true;
        }

        public void SetCursor(bool status) {
            _cursor.enabled = status;
        }

        public void Confirm()
        {
            _event.Invoke();
        }
    }
}