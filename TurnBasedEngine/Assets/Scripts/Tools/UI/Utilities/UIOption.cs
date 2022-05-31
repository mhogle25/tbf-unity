using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BF2D.Enums;
using BF2D.Attributes;

namespace BF2D.UI {
    public struct UIOptionData
    {
        public string name;
        public string text;
        public Sprite icon;
        public InputButtonCollection<Action> actions;
    };

    [RequireComponent(typeof(InputEventController))]
    public class UIOption : MonoBehaviour {
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image image = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;
        
        private InputEventController inputEventController = null;

        private UIOptionData data;
        private void Awake()
        {
            this.inputEventController = GetComponent<InputEventController>();
        }
        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public bool Setup(UIOptionData optionData) {
            this.gameObject.name = optionData.name;
            this.data = optionData;

            if (this.data.text != null || this.data.text == string.Empty) {
                this.textMesh.text = this.data.text;
            } else {
                if (this.textMesh.text == null)
                {
                    this.textMesh.enabled = false;
                }
            }

            if (this.data.icon != null) {
                this.image.sprite = this.data.icon;
            } else {
                if (this.image.sprite == null)
                {
                    this.image.enabled = false;
                }
            }

            foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
            {
                if (this.data.actions[inputButton] != null)
                {
                    GetEvent(inputButton).AddListener(() =>
                    {
                        this.data.actions[inputButton]();
                    });
                }
            }

            return true;
        }

        public void SetCursor(bool status) {
            this.cursor.enabled = status;
        }

        public void InvokeEvent(InputButton inputButton)
        {
            if (EventEnabled(inputButton))
                GetEvent(inputButton)?.Invoke();
        }

        private UnityEvent GetEvent(InputButton inputButton)
        {
            return this.inputEventController.GetInputEvent(inputButton);
        }

        private bool EventEnabled(InputButton inputButton)
        {
            return this.inputEventController.EventEnabled(inputButton);
        }
    }
}