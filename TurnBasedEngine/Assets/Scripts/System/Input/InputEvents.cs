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
        public bool ConfirmEnabled { get => this.confirmEnabled; set => this.confirmEnabled = value; }
        public UnityEvent ConfirmEvent => this.confirmEvent;
        [SerializeField] protected bool confirmEnabled = true;
        [Tooltip("The actions that will be called on confirm")]
        [SerializeField] protected UnityEvent confirmEvent = new();

        public bool BackEnabled { get => this.backEnabled;  set => this.backEnabled = value; }
        public UnityEvent BackEvent => this.backEvent;
        [SerializeField] protected bool backEnabled = true;
        [Tooltip("The actions that will be called on back")]
        [SerializeField] protected UnityEvent backEvent = new();

        public bool MenuEnabled { get => this.menuEnabled; set => this.menuEnabled = value; }
        public UnityEvent Menuvent => this.menuEvent;
        [SerializeField] protected bool menuEnabled = true;
        [Tooltip("The actions that will be called on menu")]
        [SerializeField] protected UnityEvent menuEvent = new();

        public bool SpecialEnabled { get => this.specialEnabled; set => this.specialEnabled = value; }
        public UnityEvent SpecialEvent => this.specialEvent;
        [SerializeField] protected bool specialEnabled = true;
        [Tooltip("The actions that will be called on special")]
        [SerializeField] protected UnityEvent specialEvent = new();

        public bool PauseEnabled { get => this.pauseEnabled; set => this.pauseEnabled = value; }
        public UnityEvent PauseEvent => this.pauseEvent;
        [SerializeField] protected bool pauseEnabled = true;
        [Tooltip("The actions that will be performed on pause")]
        [SerializeField] protected UnityEvent pauseEvent = new();

        public bool SelectEnabled { get => this.selectEnabled; set => this.selectEnabled = value; }
        public UnityEvent SelectEvent => this.selectEvent;
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
                _ => throw new ArgumentException("[InputEvents:GetInputEvent] InputButton was null or invalid")
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
                _ => throw new ArgumentException("[InputEvents:InputEventEnabled] InputButton was null or invalid")
            };
        }
    }

}
