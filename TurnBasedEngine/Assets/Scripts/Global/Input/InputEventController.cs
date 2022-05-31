using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BF2D.Enums;
using BF2D.Attributes;

namespace BF2D
{
    public class InputEventController : MonoBehaviour
    {
        public bool Listening { get { return this.listening; } set { this.listening = value; } }
        [SerializeField] private bool listening = false;
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

        private void Update()
        {
            if (!this.listening)
                return;

            if (InputManager.ConfirmPress && this.confirmEnabled)
            {
                this.confirmEvent.Invoke();
            }

            if (InputManager.MenuPress && this.menuEnabled)
            {
                this.menuEvent.Invoke();
            }

            if (InputManager.AttackPress && this.attackEnabled)
            {
                this.attackEvent.Invoke();
            }

            if (InputManager.BackPress && this.backEnabled)
            {
                this.backEvent.Invoke();
            }

            if (InputManager.PausePress && this.pauseEnabled)
            {
                this.pauseEvent.Invoke();
            }

            if (InputManager.SelectPress && this.selectEnabled)
            {
                this.selectEvent.Invoke();
            }
        }

        public UnityEvent GetInputEvent(InputButton inputButton)
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

        public bool EventEnabled(InputButton inputButton)
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
