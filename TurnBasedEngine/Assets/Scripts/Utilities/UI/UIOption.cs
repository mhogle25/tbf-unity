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
        public Action action;
    };

    public class UIOption : MonoBehaviour {
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image image = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent confirmEvent = new UnityEvent();

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public bool Setup(UIOptionData optionData) {
            if (optionData.text != null) {
                this.textMesh.text = optionData.text;
            } else {
                if (this.textMesh != null)
                {
                    this.textMesh.enabled = false;
                }
            }

            if (optionData.icon != null) {
                this.image.sprite = optionData.icon;
            } else {
                if (this.image != null)
                {
                    this.image.enabled = false;
                }
            }

            if (optionData.action != null)
            {
                this.confirmEvent.AddListener(() =>
                {
                    optionData.action();
                });
            }

            return true;
        }

        public void SetCursor(bool status) {
            //Debug.Log(gameObject.name);
            this.cursor.enabled = status;
        }

        public void Confirm()
        {
            this.confirmEvent.Invoke();
        }
    }
}