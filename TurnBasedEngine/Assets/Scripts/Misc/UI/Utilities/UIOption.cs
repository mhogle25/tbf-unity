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

    public class UIOption : MonoBehaviour {
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image image = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;

        public bool ConfirmEnabled { get { return this.confirmEnabled; } set { this.confirmEnabled = value; } }
        [SerializeField] private bool confirmEnabled = true;
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] private UnityEvent confirmEvent = new UnityEvent();
        public bool BackEnabled { get { return this.backEnabled; } set { this.backEnabled = value; } }
        [SerializeField] private bool backEnabled = true;
        [Tooltip("The actions that will be called on back")]
        [SerializeField] private UnityEvent backEvent = new UnityEvent();
        public bool MenuEnabled { get { return this.menuEnabled; } set { this.menuEnabled = value; } }
        [SerializeField] private bool menuEnabled = true;
        [Tooltip("The actions that will be called on menu")]
        [SerializeField] private UnityEvent menuEvent = new UnityEvent();
        public bool AttackEnabled { get { return this.attackEnabled; } set { this.attackEnabled = value; } }
        [SerializeField] private bool attackEnabled = true;
        [Tooltip("The actions that will be called on attack")]
        [SerializeField] private UnityEvent attackEvent = new UnityEvent();
        public bool PauseEnabled { get { return this.pauseEnabled; } set { this.pauseEnabled = value; } }
        [SerializeField] private bool pauseEnabled = true;
        [Tooltip("The actions that will be performed on pause")]
        [SerializeField] private UnityEvent pauseEvent = new UnityEvent();
        public bool SelectEnabled { get { return this.selectEnabled; } set { this.selectEnabled = value; } }
        [SerializeField] private bool selectEnabled = true;
        [Tooltip("The actions that will be performed on select")]
        [SerializeField] private UnityEvent selectEvent = new UnityEvent();

        private UIOptionData data;

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public bool Setup(UIOptionData optionData) {
            this.gameObject.name = optionData.name;
            this.data = optionData;

            if (this.data.text != null) {
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
            return inputButton switch
            {
                InputButton.Confirm => this.confirmEvent,
                InputButton.Back => this.backEvent,
                InputButton.Menu => this.menuEvent,
                InputButton.Attack => this.attackEvent,
                InputButton.Pause => this.pauseEvent,
                InputButton.Select => this.selectEvent,
                _ => throw new ArgumentException("[UIOption] InputButton was null or invalid")
            };
        }

        private bool EventEnabled(InputButton inputButton)
        {
            return inputButton switch
            {
                InputButton.Confirm => this.confirmEnabled,
                InputButton.Back => this.backEnabled,
                InputButton.Menu => this.menuEnabled,
                InputButton.Attack => this.attackEnabled,
                InputButton.Pause => this.pauseEnabled,
                InputButton.Select => this.selectEnabled,
                _ => throw new ArgumentException("[UIOption] InputButton was null or invalid")
            };
        }
    }
}