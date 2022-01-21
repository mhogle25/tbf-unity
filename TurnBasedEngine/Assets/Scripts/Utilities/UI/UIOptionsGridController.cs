using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BF2D.UI
{
    public class UIOptionsGridController : MonoBehaviour
    {

        [SerializeField] private UIOptionsGrid optionsGrid = null;

        public UIOptionsGrid OptionsGrid
        {
            get
            {
                return this.optionsGrid;
            }

            set
            {
                this.optionsGrid = value;
            }
        }

        [SerializeField] private float delay = 0.5f;

        public float Delay
        {
            get
            {
                return this.delay;
            }

            set
            {
                this.delay = value;
            }
        }

        [SerializeField] private float speed = 0.1f;

        public float Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = value;
            }
        }

        private Action state = null;
        private MoveDirection direction = MoveDirection.Left;
        private float time = 0f;

        private void Awake()
        {
            this.state = StateInputListener;
        }

        private void Update()
        {
            if (this.state != null)
            {
                this.state();
            }
        }

        private void StateInputListener()
        {
            if (InputManager.LeftPress)
            {
                DirectionalCall(MoveDirection.Left);
            }

            if (InputManager.UpPress)
            {
                DirectionalCall(MoveDirection.Up);
            }

            if (InputManager.RightPress)
            {
                DirectionalCall(MoveDirection.Right);
            }

            if (InputManager.DownPress)
            {
                DirectionalCall(MoveDirection.Down);
            }

            if (InputManager.ConfirmPress)
            {
                this.optionsGrid.Confirm();
            }
        }

        private void StateDelay()
        {
            KeyReleaseListener();

            if (Time.time > this.time + delay)
            {
                this.time = 0f;
                this.state = StateAutoNavigate;
            }
        }

        private void StateAutoNavigate()
        {
            KeyReleaseListener();

            if (Time.time > this.time)
            {
                this.time = Time.time + this.speed;
                this.optionsGrid.Navigate(this.direction);
            }
        }

        private void KeyReleaseListener()
        {
            if (InputReleaseFromDirection(this.direction))
            {
                this.state = StateInputListener;
            }
        }

        private void DirectionalCall(MoveDirection direction)
        {
            this.optionsGrid.Navigate(direction);
            this.direction = direction;
            this.time = Time.time;
            this.state = StateDelay;
        }

        private bool InputReleaseFromDirection(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.Left:
                    return InputManager.LeftRelease;
                case MoveDirection.Up:
                    return InputManager.UpRelease;
                case MoveDirection.Right:
                    return InputManager.RightRelease;
                case MoveDirection.Down:
                    return InputManager.DownRelease;
                default:
                    Debug.LogError("[UIOptionsGridController] Invalid move direction");
                    return false;
            }
        }
    }
}
