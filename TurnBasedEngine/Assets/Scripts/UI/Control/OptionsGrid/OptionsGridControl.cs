using System;
using UnityEngine;
using BF2D.Enums;

namespace BF2D.UI 
{ 
    public class OptionsGridControl : UIControl
    {
        public OptionsGrid Controlled { get => this.controlled; protected set => this.controlled = value; }
        [Header("Options Grid")]
        [SerializeField] private OptionsGrid controlled = null;

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

            if (InputCtx.One.ConfirmPress)
                this.controlled.InvokeEvent(InputButton.Confirm);

            if (InputCtx.One.MenuPress)
                this.controlled.InvokeEvent(InputButton.Menu);

            if (InputCtx.One.SpecialPress)
                this.controlled.InvokeEvent(InputButton.Special);

            if (InputCtx.One.BackPress)
                this.controlled.InvokeEvent(InputButton.Back);

            if (InputCtx.One.PausePress)
                this.controlled.InvokeEvent(InputButton.Pause);

            if (InputCtx.One.SelectPress)
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
            if (InputCtx.One.Left)
                DirectionalCall(InputDirection.Left);

            if (InputCtx.One.Up)
                DirectionalCall(InputDirection.Up);

            if (InputCtx.One.Right)
                DirectionalCall(InputDirection.Right);

            if (InputCtx.One.Down)
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

        private bool InputReleaseFromDirection(InputDirection InputDirection) => InputDirection switch
        {
            InputDirection.Left => !InputCtx.One.Left,
            InputDirection.Up => !InputCtx.One.Up,
            InputDirection.Right => !InputCtx.One.Right,
            InputDirection.Down => !InputCtx.One.Down,
            _ => throw new ArgumentException("[OptionsGridControl:InputReleaseFromDirection] The given InputDirection was null or invalid"),
        };
    }
}
