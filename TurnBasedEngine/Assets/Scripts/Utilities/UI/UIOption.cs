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
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private Image _image;
        [SerializeField] private Image _cursor;
        [SerializeField] private UnityEvent _event = new UnityEvent();

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