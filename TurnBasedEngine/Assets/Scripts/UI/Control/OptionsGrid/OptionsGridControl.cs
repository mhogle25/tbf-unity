using System;
using UnityEngine;
using BF2D.Enums;

namespace BF2D.UI 
{ 
    public class OptionsGridControl : UIControl
    {
        public OptionsGrid Controlled => this.controlled;
        [Header("Options Grid")]
        [SerializeField] protected OptionsGrid controlled = null;

        public float Delay { get => this.delay; set => this.delay = value; }
        [SerializeField] private float delay = 0.5f;

        public float Speed { get => this.speed; set => this.speed = value; }
        [SerializeField] private float speed = 0.1f;

        private InputDirection direction = InputDirection.Left;
        private float timeAccumulator = 0f;

        protected Action state = null;

        protected virtual void Awake()
        {
            this.state = StateDirectionInputListener;
        }

        protected virtual void Update()
        {
            if (!this.controlled)
                return;

            this.state?.Invoke();

            if (InputManager.Instance.ConfirmPress)
                this.controlled.InvokeEvent(InputButton.Confirm);

            if (InputManager.Instance.MenuPress)
                this.controlled.InvokeEvent(InputButton.Menu);

            if (InputManager.Instance.SpecialPress)
                this.controlled.InvokeEvent(InputButton.Special);

            if (InputManager.Instance.BackPress)
                this.controlled.InvokeEvent(InputButton.Back);

            if (InputManager.Instance.PausePress)
                this.controlled.InvokeEvent(InputButton.Pause);

            if (InputManager.Instance.SelectPress)
                this.controlled.InvokeEvent(InputButton.Select);
        }

        public override void ControlInitialize()
        {
            this.controlled.UtilityInitialize();
        }

        public override void ControlFinalize()
        {
            this.controlled.UtilityFinalize();
        }

        private void StateDirectionInputListener()
        {
            if (InputManager.Instance.Left)
                DirectionalCall(InputDirection.Left);

            if (InputManager.Instance.Up)
                DirectionalCall(InputDirection.Up);

            if (InputManager.Instance.Right)
                DirectionalCall(InputDirection.Right);

            if (InputManager.Instance.Down)
                DirectionalCall(InputDirection.Down);
        }

        private void StateDelay()
        {
            KeyReleaseListener();

            if (Time.time > this.timeAccumulator + delay)
            {
                this.timeAccumulator = 0f;
                this.state = StateAutoNavigate;
            }
        }

        private void StateAutoNavigate()
        {
            KeyReleaseListener();

            if (Time.time > this.timeAccumulator)
            {
                this.timeAccumulator = Time.time + this.speed;
                this.controlled.Navigate(this.direction);
            }
        }

        private void KeyReleaseListener()
        {
            if (InputReleaseFromDirection(this.direction))
                this.state = StateDirectionInputListener;
        }

        private void DirectionalCall(InputDirection direction)
        {
            this.controlled.Navigate(direction);
            this.direction = direction;
            this.timeAccumulator = Time.time;
            this.state = StateDelay;
        }

        private bool InputReleaseFromDirection(InputDirection InputDirection)
        {
            return InputDirection switch
            {
                InputDirection.Left => !InputManager.Instance.Left,
                InputDirection.Up => !InputManager.Instance.Up,
                InputDirection.Right => !InputManager.Instance.Right,
                InputDirection.Down => !InputManager.Instance.Down,
                _ => false,
            };
        }
    }
}
