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
        public Action confirmAction;
        public Action menuAction;
        public Action attackAction;
        public Action backAction;
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
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent menuEvent = new UnityEvent();
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent attackEvent = new UnityEvent();
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent backEvent = new UnityEvent();

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

            if (optionData.confirmAction != null)
            {
                this.confirmEvent.AddListener(() =>
                {
                    optionData.confirmAction();
                });
            }

            if (optionData.menuAction != null)
            {
                this.menuEvent.AddListener(() =>
                {
                    optionData.menuAction();
                });
            }

            if (optionData.attackAction != null)
            {
                this.attackEvent.AddListener(() =>
                {
                    optionData.attackAction();
                });
            }

            if (optionData.backAction != null)
            {
                this.backEvent.AddListener(() =>
                {
                    optionData.backAction();
                });
            }

            return true;
        }

        public void SetCursor(bool status) {
            //Debug.Log(gameObject.name);
            this.cursor.enabled = status;
        }

        public void ConfirmInvoke()
        {
            this.confirmEvent.Invoke();
        }

        public void MenuInvoke()
        {
            this.menuEvent.Invoke();
        }

        public void AttackInvoke()
        {
            this.attackEvent.Invoke();
        }

        public void BackInvoke()
        {
            this.backEvent.Invoke();
        }
    }
}