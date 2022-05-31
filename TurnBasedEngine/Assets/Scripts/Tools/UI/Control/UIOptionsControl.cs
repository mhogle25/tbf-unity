using System;
using UnityEngine;
using BF2D.Enums;

namespace BF2D.UI
{
    public class UIOptionsControl : UIControl
    {
        public UIOptionsGrid OptionsGrid { get { return this.optionsGrid; } }
        [SerializeField] protected UIOptionsGrid optionsGrid = null;

        public float Delay { get { return this.delay; } set { this.delay = value; } }
        [SerializeField] private float delay = 0.5f;

        public float Speed { get { return this.speed; } set { this.speed = value; } }
        [SerializeField] private float speed = 0.1f;

        private InputDirection direction = InputDirection.Left;
        private float timeAccumulator = 0f;

        private Action state = null;

        protected virtual void Awake()
        {
            this.state = StateDirectionInputListener;
        }

        protected virtual void Update()
        {
            if (!this.Interactable)
                return;

            this.state?.Invoke();

            if (InputManager.ConfirmPress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Confirm);
            }

            if (InputManager.MenuPress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Menu);
            }

            if (InputManager.AttackPress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Attack);
            }

            if (InputManager.BackPress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Back);
            }

            if (InputManager.PausePress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Pause);
            }

            if (InputManager.SelectPress)
            {
                this.optionsGrid.InvokeEvent(InputButton.Select);
            }
        }

        public override void ControlInitialize()
        {
            this.optionsGrid.UtilityInitialize();
        }

        public override void ControlFinalize()
        {
            this.optionsGrid.UtilityFinalize();
        }

        private void StateDirectionInputListener()
        {
            if (InputManager.Left)
            {
                DirectionalCall(InputDirection.Left);
            }

            if (InputManager.Up)
            {
                DirectionalCall(InputDirection.Up);
            }

            if (InputManager.Right)
            {
                DirectionalCall(InputDirection.Right);
            }

            if (InputManager.Down)
            {
                DirectionalCall(InputDirection.Down);
            }
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
                this.optionsGrid.Navigate(this.direction);
            }
        }

        private void KeyReleaseListener()
        {
            if (InputReleaseFromDirection(this.direction))
            {
                this.state = StateDirectionInputListener;
            }
        }

        private void DirectionalCall(InputDirection direction)
        {
            this.optionsGrid.Navigate(direction);
            this.direction = direction;
            this.timeAccumulator = Time.time;
            this.state = StateDelay;
        }

        private bool InputReleaseFromDirection(InputDirection InputDirection)
        {
            switch (InputDirection)
            {
                case InputDirection.Left:
                    return !InputManager.Left;
                case InputDirection.Up:
                    return !InputManager.Up;
                case InputDirection.Right:
                    return !InputManager.Right;
                case InputDirection.Down:
                    return !InputManager.Down;
                default:
                    Debug.LogError("[UIOptionsGridController] Invalid move direction");
                    return false;
            }
        }
    }
}
