using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BF2D.Enums;
using BF2D.Attributes;

namespace BF2D
{
    public class InputEvents : MonoBehaviour
    {
        public bool ConfirmEnabled { get { return this.confirmEnabled; } set { this.confirmEnabled = value; } }
        [SerializeField] protected bool confirmEnabled = true;
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] protected UnityEvent confirmEvent = new();
        public bool BackEnabled { get { return this.backEnabled; } set { this.backEnabled = value; } }
        [SerializeField] protected bool backEnabled = true;
        [Tooltip("The actions that will be called on back")]
        [SerializeField] protected UnityEvent backEvent = new();
        public bool MenuEnabled { get { return this.menuEnabled; } set { this.menuEnabled = value; } }
        [SerializeField] protected bool menuEnabled = true;
        [Tooltip("The actions that will be called on menu")]
        [SerializeField] protected UnityEvent menuEvent = new();
        public bool SpecialEnabled { get { return this.specialEnabled; } set { this.specialEnabled = value; } }
        [SerializeField] protected bool specialEnabled = true;
        [Tooltip("The actions that will be called on attack")]
        [SerializeField] protected UnityEvent specialEvent = new();
        public bool PauseEnabled { get { return this.pauseEnabled; } set { this.pauseEnabled = value; } }
        [SerializeField] protected bool pauseEnabled = true;
        [Tooltip("The actions that will be performed on pause")]
        [SerializeField] protected UnityEvent pauseEvent = new();
        public bool SelectEnabled { get { return this.selectEnabled; } set { this.selectEnabled = value; } }
        [SerializeField] protected bool selectEnabled = true;
        [Tooltip("The actions that will be performed on select")]
        [SerializeField] protected UnityEvent selectEvent = new();

        public UnityEvent GetInputEvent(InputButton inputButton)
        {

            return inputButton switch
            {
                InputButton.Confirm => this.confirmEvent,
                InputButton.Back => this.backEvent,
                InputButton.Menu => this.menuEvent,
                InputButton.Special => this.specialEvent,
                InputButton.Pause => this.pauseEvent,
                InputButton.Select => this.selectEvent,
                _ => throw new ArgumentException("[UIOption] InputButton was null or invalid")
            };
        }

        public bool InputEventEnabled(InputButton inputButton)
        {
            return inputButton switch
            {
                InputButton.Confirm => this.confirmEnabled,
                InputButton.Back => this.backEnabled,
                InputButton.Menu => this.menuEnabled,
                InputButton.Special => this.specialEnabled,
                InputButton.Pause => this.pauseEnabled,
                InputButton.Select => this.selectEnabled,
                _ => throw new ArgumentException("[UIOption] InputButton was null or invalid")
            };
        }
    }

}
