using System;
using UnityEngine;
using BF2D.Enums;
using System.Collections.Generic;

namespace BF2D.UI 
{ 
    public class OptionsGridControl : UIControl
    {
        public OptionsGrid Controlled { get { return this.controlled; } }
        [Header("Options Grid")]
        [SerializeField] protected OptionsGrid controlled = null;

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
            if (!this.controlled)
                return;

            this.state?.Invoke();

            if (InputManager.Instance.ConfirmPress)
            {
                this.controlled.InvokeEvent(InputButton.Confirm);
            }

            if (InputManager.Instance.MenuPress)
            {
                this.controlled.InvokeEvent(InputButton.Menu);
            }

            if (InputManager.Instance.AttackPress)
            {
                this.controlled.InvokeEvent(InputButton.Attack);
            }

            if (InputManager.Instance.BackPress)
            {
                this.controlled.InvokeEvent(InputButton.Back);
            }

            if (InputManager.Instance.PausePress)
            {
                this.controlled.InvokeEvent(InputButton.Pause);
            }

            if (InputManager.Instance.SelectPress)
            {
                this.controlled.InvokeEvent(InputButton.Select);
            }
        }

        public override void ControlInitialize()
        {
            this.controlled.UtilityInitialize();
        }

        public override void ControlFinalize()
        {
            this.controlled.UtilityFinalize();
        }

        protected void LoadOptionsIntoGrid(OptionsGrid grid, List<GridOption> initGridOptions)
        {
            if (grid.Width > 0 && grid.Height > 0)
            {
                //Create the element data structure
                grid.Setup(grid.Width, grid.Height);
            }

            if (initGridOptions.Count > 0)
            {
                foreach (GridOption option in initGridOptions)
                {
                    grid.Add(option);
                }
            }
        }

        private void StateDirectionInputListener()
        {
            if (InputManager.Instance.Left)
            {
                DirectionalCall(InputDirection.Left);
            }

            if (InputManager.Instance.Up)
            {
                DirectionalCall(InputDirection.Up);
            }

            if (InputManager.Instance.Right)
            {
                DirectionalCall(InputDirection.Right);
            }

            if (InputManager.Instance.Down)
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
                this.controlled.Navigate(this.direction);
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
            this.controlled.Navigate(direction);
            this.direction = direction;
            this.timeAccumulator = Time.time;
            this.state = StateDelay;
        }

        private bool InputReleaseFromDirection(InputDirection InputDirection)
        {
            switch (InputDirection)
            {
                case InputDirection.Left:
                    return !InputManager.Instance.Left;
                case InputDirection.Up:
                    return !InputManager.Instance.Up;
                case InputDirection.Right:
                    return !InputManager.Instance.Right;
                case InputDirection.Down:
                    return !InputManager.Instance.Down;
                default:
                    Terminal.IO.LogError("[UIOptionsGridController] Invalid move direction");
                    return false;
            }
        }
    }
}
