using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BF2D.Enums;
using BF2D.Attributes;

namespace BF2D.UI {

    public class UIOption : InputEvents {
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image icon = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;

        public struct Data
        {
            public string name;
            public string text;
            public Sprite icon;
            public InputButtonCollection<Action> actions;
        };

        public Data GetData { get { return this.data; } }
        private Data data;

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public bool Setup(Data optionData) {
            this.gameObject.name = optionData.name;
            this.data = optionData;

            SetupText(this.data.text);
            SetupIcon(this.data.icon);

            foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
            {
                if (this.data.actions[inputButton] != null)
                {
                    GetInputEvent(inputButton).AddListener(() =>
                    {
                        this.data.actions[inputButton]();
                    });
                }
            }

            return true;
        }

        private void SetupText(string text)
        {
            if (this.textMesh is null)
                return;

            if (text != null || text != string.Empty)
            {
                this.textMesh.text = text;
            }
            else
            {
                if (this.textMesh.text == null)
                {
                    this.textMesh.enabled = false;
                }
            }
        }

        private void SetupIcon(Sprite icon)
        {
            if (this.icon is null)
                return;

            if (icon != null)
            {
                this.icon.sprite = icon;
            }
            else
            {
                if (this.icon.sprite == null)
                {
                    this.icon.enabled = false;
                }
            }
        }

        public void SetCursor(bool status) {
            this.cursor.enabled = status;
        }

        public void InvokeEvent(InputButton inputButton)
        {
            if (InputEventEnabled(inputButton))
                GetInputEvent(inputButton)?.Invoke();
        }
    }
}