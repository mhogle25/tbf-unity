using BF2D.Enums;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BF2D
{
    public class InputCtx : MonoBehaviourSingleton<InputCtx>
    {
        [Serializable]
        public abstract class ControlsConfig
        {
            [JsonIgnore] public abstract string ID { get; set; }

            [JsonIgnore] public abstract InputController ControlType { get; }

            [JsonIgnore] public KeyCode Confirm { get => this.confirm; set => this.confirm = value; }
            [JsonProperty] private protected KeyCode confirm = KeyCode.P;
            [JsonIgnore] public KeyCode Back { get => this.back; set => this.back = value; }
            [JsonProperty] private protected KeyCode back = KeyCode.O;
            [JsonIgnore] public KeyCode Menu { get => this.menu; set => this.menu = value; }
            [JsonProperty] private protected KeyCode menu = KeyCode.I;
            [JsonIgnore] public KeyCode Special { get => this.special; set => this.special = value; }
            [JsonProperty] private protected KeyCode special = KeyCode.U;
            [JsonIgnore] public KeyCode Pause { get => this.pause; set => this.pause = value; }
            [JsonProperty] private protected KeyCode pause = KeyCode.Escape;
            [JsonIgnore] public KeyCode Select { get => this.select; set => this.select = value; }
            [JsonProperty] private protected KeyCode select = KeyCode.Tab;

            private KeyCode GetButtonKeyCode(InputButton inputButton)
            {
                return inputButton switch
                {
                    InputButton.Confirm => this.confirm,
                    InputButton.Back => this.back,
                    InputButton.Menu => this.menu,
                    InputButton.Special => this.special,
                    InputButton.Pause => this.pause,
                    InputButton.Select => this.select,
                    _ => throw new ArgumentException("[InputManager:ControlsConfig:GetButtonKeyCode] InputButton was null or invalid"),
                };
            }

            public bool GetButton(InputButton inputButton)
            {
                return Input.GetKey(GetButtonKeyCode(inputButton));
            }

            public bool GetButtonPress(InputButton inputButton)
            {
                return Input.GetKeyDown(GetButtonKeyCode(inputButton));
            }

            public bool GetButtonRelease(InputButton inputButton)
            {
                return Input.GetKeyUp(GetButtonKeyCode(inputButton));
            }

            public abstract bool GetDirection(InputDirection inputDirection);
            public abstract float GetAxis(Axis axis);
        }

        [Serializable]
        public class ControlsConfigKeyboard : ControlsConfig
        {
            [JsonIgnore] public override string ID { get => this.id; set => this.id = value; } 
            [JsonProperty] private string id = "default";

            [JsonIgnore] public override InputController ControlType => ControlsConfigKeyboard.controlType;
            private const InputController controlType = InputController.Keyboard;

            [JsonIgnore] public KeyCode Up { get => this.up; set => this.up = value; }
            [JsonProperty] private KeyCode up = KeyCode.W;
            [JsonIgnore] public KeyCode Left { get => this.left; set => this.left = value; }
            [JsonProperty] private KeyCode left = KeyCode.A;
            [JsonIgnore] public KeyCode Down { get => this.down; set => this.down = value; }
            [JsonProperty] private KeyCode down = KeyCode.S;
            [JsonIgnore] public KeyCode Right { get => this.right; set => this.right = value; }
            [JsonProperty] private KeyCode right = KeyCode.D;

            private KeyCode GetDirectionKeyCode(InputDirection inputDirection)
            {
                return inputDirection switch
                {
                    InputDirection.Up => this.up,
                    InputDirection.Left => this.left,
                    InputDirection.Down => this.down,
                    InputDirection.Right => this.right,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigKeyboard:GetDirectionKeyCode] InputButton was null or invalid"),
                };
            }

            public override bool GetDirection(InputDirection inputDirection)
            {
                return Input.GetKey(GetDirectionKeyCode(inputDirection));
            }

            public override float GetAxis(Axis axis)
            {
                //Set Horizontal and Vertical Axis Values
                float tempHor;
                float tempVer;
                if (Input.GetKey(this.left) && Input.GetKey(this.right))
                {
                    tempHor = 0;
                }
                else if (Input.GetKey(this.left))
                {
                    tempHor = -1;
                }
                else if (Input.GetKey(this.right))
                {
                    tempHor = 1;
                }
                else
                {
                    tempHor = 0;
                }

                if (Input.GetKey(this.up) && Input.GetKey(this.down))
                {
                    tempVer = 0;
                }
                else if (Input.GetKey(this.down))
                {
                    tempVer = -1;
                }
                else if (Input.GetKey(this.up))
                {
                    tempVer = 1;
                }
                else
                {
                    tempVer = 0;
                }

                //Calculate distances for diagonals (unit circle shit)
                if (tempVer != 0 && tempHor != 0)
                {
                    tempHor *= 0.7071f; // 0.707106781 is an approximation of sqrt(2)/2
                    tempVer *= 0.7071f;
                }

                return axis switch
                {
                    Axis.Horizontal => tempHor,
                    Axis.Vertical => tempVer,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigKeyboard:GetAxis] Axis was null or invalid"),
                };
            }

            /// <summary>
            /// Creates a copy of a Keyboard Config
            /// </summary>
            /// <returns>The copy of the Keyboard Config</returns>
            public ControlsConfigKeyboard Clone()
            {
                return new ControlsConfigKeyboard
                {
                    Up = this.Up,
                    Left = this.Left,
                    Down = this.Down,
                    Right = this.Right,
                    Confirm = this.Confirm,
                    Back = this.Back,
                    Menu = this.Menu,
                    Special = this.Special,
                    Pause = this.Pause,
                    Select = this.Select
                };
            }
        }

        [Serializable]
        public class ControlsConfigGamepad : ControlsConfig
        {
            #region AXIS_KEYS
            private const string HORIZONTAL_AXIS = "Horizontal";
            private const string VERTICAL_AXIS = "Vertical";
            private const string D_HORIZONTAL_AXIS = "D Horizontal";
            private const string D_VERTICAL_AXIS = "D Vertical";
            #endregion

            [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
            [JsonProperty] private string id = "default";

            [JsonIgnore] public override InputController ControlType => ControlsConfigGamepad.controlType;
            private const InputController controlType = InputController.Gamepad;
            [JsonIgnore] public float JoystickThreshold { get => this.joystickThreshold; set => this.joystickThreshold = value; }
            [JsonProperty] private float joystickThreshold = 0.3f;

            //Initialize Gamepad default values
            public ControlsConfigGamepad()
            {
                this.confirm = KeyCode.JoystickButton0;
                this.back = KeyCode.JoystickButton1;
                this.menu = KeyCode.JoystickButton3;
                this.special = KeyCode.JoystickButton2;
                this.pause = KeyCode.JoystickButton7;
                this.select = KeyCode.JoystickButton6;
            }

            private string GetAxisKey(Axis axis)
            {
                return axis switch
                {
                    Axis.Horizontal => ControlsConfigGamepad.HORIZONTAL_AXIS,
                    Axis.Vertical => ControlsConfigGamepad.VERTICAL_AXIS,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigGamepad:GetAxisKey] Axis was null or invalid"),
                };
            }

            private string GetDAxisKey(Axis axis)
            {
                return axis switch
                {
                    Axis.Horizontal => ControlsConfigGamepad.D_HORIZONTAL_AXIS,
                    Axis.Vertical => ControlsConfigGamepad.D_VERTICAL_AXIS,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigGamepad:GetAxisKey] Axis was null or invalid"),
                };
            }

            private float MaxAxis(float axisResult, float dAxisResult)
            {
                if (Mathf.Abs(axisResult) < this.joystickThreshold && Mathf.Abs(dAxisResult) < this.joystickThreshold)
                {
                    return 0f;
                }
                else if (Mathf.Abs(dAxisResult) < this.joystickThreshold)
                {
                    return axisResult;
                }
                else
                {
                    return dAxisResult;
                }
            }

            public override bool GetDirection(InputDirection inputDirection)
            {
                return inputDirection switch
                {
                    InputDirection.Up => MaxAxis(Input.GetAxis(GetAxisKey(Axis.Vertical)), Input.GetAxis(GetDAxisKey(Axis.Vertical))) > this.joystickThreshold,
                    InputDirection.Left => MaxAxis(Input.GetAxis(GetAxisKey(Axis.Horizontal)), Input.GetAxis(GetDAxisKey(Axis.Horizontal))) < -this.joystickThreshold,
                    InputDirection.Down => MaxAxis(Input.GetAxis(GetAxisKey(Axis.Vertical)), Input.GetAxis(GetDAxisKey(Axis.Vertical))) < -this.joystickThreshold,
                    InputDirection.Right => MaxAxis(Input.GetAxis(GetAxisKey(Axis.Horizontal)), Input.GetAxis(GetDAxisKey(Axis.Horizontal))) > this.joystickThreshold,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigGamepad:GetDirection] InputDirection was null or invalid"),
                };
            }

            public override float GetAxis(Axis axis)
            {
                return MaxAxis(Input.GetAxis(GetAxisKey(axis)), Input.GetAxis(GetDAxisKey(axis)));
            }

            /// <summary>
            /// Creates a copy of a Gamepad Config
            /// </summary>
            /// <param iconID="config">The Gamepad Config to be copied</param>
            /// <returns>The copy of the Gamepad Config</returns>
            public ControlsConfigGamepad Clone()
            {
                return new ControlsConfigGamepad
                {
                    Confirm = this.Confirm,
                    Back = this.Back,
                    Menu = this.Menu,
                    Special = this.Special,
                    Pause = this.Pause,
                    Select = this.Select,
                    JoystickThreshold = this.JoystickThreshold
                };
            }
        }

        [Header("Input")]
        [SerializeField] private List<KeyCode> blacklistedCharacters = new();

        public bool InputEnabled { get => this.inputEnabled; set => this.inputEnabled = value; }
        private bool inputEnabled = true;

        private readonly ControlsConfigKeyboard defaultKeyboardConfig;
        private readonly ControlsConfigGamepad defaultGamepadConfig;
        private ControlsConfigKeyboard keyboardConfig;
        private ControlsConfigGamepad gamepadConfig;

        private ControlsConfig currentConfig;
        private KeyCode lastHitKey;

        private Action states;
        private Action buttonEvent;
        private Action directionEvent;

        public bool Up => GetDirection(InputDirection.Up);
        public bool Left => GetDirection(InputDirection.Left);
        public bool Down => GetDirection(InputDirection.Down);
        public bool Right => GetDirection(InputDirection.Right);
        public bool Confirm => GetButton(InputButton.Confirm);
        public bool Back => GetButton(InputButton.Back);
        public bool Menu => GetButton(InputButton.Menu);
        public bool Special => GetButton(InputButton.Special);
        public bool Pause => GetButton(InputButton.Pause);
        public bool Select => GetButton(InputButton.Select);
        public bool ConfirmPress => GetButtonPress(InputButton.Confirm);
        public bool BackPress => GetButtonPress(InputButton.Back);
        public bool MenuPress => GetButtonPress(InputButton.Menu);
        public bool SpecialPress => GetButtonPress(InputButton.Special);
        public bool PausePress => GetButtonPress(InputButton.Pause);
        public bool SelectPress => GetButtonPress(InputButton.Select);
        public bool ConfirmRelease => GetButtonRelease(InputButton.Confirm);
        public bool BackRelease => GetButtonRelease(InputButton.Back);
        public bool MenuRelease => GetButtonRelease(InputButton.Menu);
        public bool SpecialRelease => GetButtonRelease(InputButton.Special);
        public bool PauseRelease => GetButtonRelease(InputButton.Pause);
        public bool SelectRelease => GetButtonRelease(InputButton.Select);
        public float HorizontalAxis => GetAxis(Axis.Horizontal);
        public float VerticalAxis => GetAxis(Axis.Vertical);

        public string KeyboardID { get => this.keyboardConfig.ID; set => this.keyboardConfig.ID = value; }
        public string GamepadID { get => this.gamepadConfig.ID; set => this.gamepadConfig.ID = value; }

        public InputCtx()
        {
            //Set up the default configs 
            this.defaultKeyboardConfig = new ControlsConfigKeyboard();
            this.defaultGamepadConfig = new ControlsConfigGamepad();

            //Set up the keyboard and gamepad configs
            this.keyboardConfig = this.defaultKeyboardConfig.Clone();
            this.gamepadConfig = this.defaultGamepadConfig.Clone();

            //Initialize the current config
            this.currentConfig = this.keyboardConfig;
        }

        protected sealed override void SingletonAwakened()
        {
            this.states += StateGamepadConnected;
        }

        private void Update()
        {
            this.states?.Invoke();
        }

        private void OnGUI()
        {
            if (Input.anyKeyDown)
                this.lastHitKey = Event.current.keyCode;
        }

        public bool GetDirection(InputDirection inputDirection)
        {
            if (!this.inputEnabled)
                return false;
            return this.currentConfig.GetDirection(inputDirection);
        }

        public bool GetButton(InputButton inputButton)
        {
            if (!this.inputEnabled)
                return false;
            return this.currentConfig.GetButton(inputButton);
        }

        public bool GetButtonPress(InputButton inputButton)
        {
            if (!this.inputEnabled)
                return false;
            return this.currentConfig.GetButtonPress(inputButton);
        }

        public bool GetButtonPress()
        {
            if (!this.inputEnabled)
                return false;
            return this.ConfirmPress || this.BackPress || this.MenuPress || this.SpecialPress || this.PausePress || this.SelectPress;
        }

        public bool GetButtonRelease(InputButton inputButton)
        {
            if (!this.inputEnabled)
                return false;
            return this.currentConfig.GetButtonPress(inputButton);
        }

        public bool GetButtonRelease()
        {
            if (!this.inputEnabled)
                return false;
            return this.ConfirmRelease || this.BackRelease || this.MenuRelease || this.SpecialRelease || this.PauseRelease || this.SelectRelease;
        }

        public float GetAxis(Axis axis)
        {
            if (!this.inputEnabled)
                return 0f;
            return this.currentConfig.GetAxis(axis);
        }

        /// <summary>
        /// Returns the config of the keyboard to its default values
        /// </summary>
        public void ResetConfig(InputController inputController)
        {
            switch (inputController)
            {
                case InputController.Keyboard:
                    this.keyboardConfig = this.defaultKeyboardConfig.Clone();
                    break;
                case InputController.Gamepad:
                    this.gamepadConfig = this.defaultGamepadConfig.Clone();
                    break;
                default:
                    Debug.LogError("[InputManager:ResetConfig] InputController was null or invalid");
                    return;
            }
        }

        /// <summary>
        /// Gets the type of the active Input Controller
        /// </summary>
        /// <returns>The type of the active Input Controller</returns>
        public InputController GetCurrentControlType()
        {
            return this.currentConfig.ControlType;
        }

        public string SerializeConfig(InputController controllerType)
        {
            return controllerType switch
            {
                InputController.Keyboard => Utilities.JSON.SerializeObject(this.keyboardConfig),
                InputController.Gamepad => Utilities.JSON.SerializeObject(this.gamepadConfig),
                _ => string.Empty,
            };
        }

        public void DeserializeConfig(InputController controllerType, string json)
        {
            switch (controllerType)
            {
                case InputController.Keyboard:
                    this.keyboardConfig = Utilities.JSON.DeserializeString<ControlsConfigKeyboard>(json);
                    break;
                case InputController.Gamepad:
                    this.gamepadConfig = Utilities.JSON.DeserializeString<ControlsConfigGamepad>(json);
                    break;
                default:
                    Debug.LogError("[InputManager:DeserializeConfig] InputController was null or invalid");
                    break;
            }

            ReloadCurrentConfig();
        }

        public void SetCurrentConfigButton(InputButton inputButton)
        {
            this.buttonEvent = () =>
            {
                StateSetCurrentConfigButton(inputButton);
            };
            this.states += this.buttonEvent;
        }

        public void SetKeyboardDirection(InputDirection inputDirection)
        {
            this.directionEvent = () =>
            {
                StateSetKeyboardDirection(inputDirection);
            };
            this.states += this.directionEvent;
        }

        private void ReloadCurrentConfig()
        {
            this.currentConfig = this.currentConfig.ControlType switch
            {
                InputController.Keyboard => this.keyboardConfig,
                InputController.Gamepad => this.gamepadConfig,
                _ => null,
            };

            if (this.currentConfig is null)
                Debug.LogError("[InputManager:ReloadCurrentConfig] InputController was null or invalid");
        }

        private void StateGamepadConnected()
        {
            string[] gamepadNames = Input.GetJoystickNames();
            if (gamepadNames.Length < 1)
            {
                return;
            }

            bool gamepadConnected = false;
            //Check if gamepad is connected
            foreach (string s in gamepadNames)
            {
                if (s != string.Empty)
                {
                    gamepadConnected = true;
                }
            }

            if (gamepadConnected)
            {
                if (GetCurrentControlType() != InputController.Gamepad)
                {
                    this.currentConfig = this.gamepadConfig;
                }
            }
            else
            {
                if (GetCurrentControlType() == InputController.Gamepad)
                {
                    this.currentConfig = this.keyboardConfig;
                }
            }
        }

        private void StateSetCurrentConfigButton(InputButton inputButton)
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            if (IsKeyCodeBlacklisted(this.lastHitKey))
            {
                Debug.LogWarning($"[InputManager:StateSetCurrentConfigButton] Tried to bind key '{this.lastHitKey}' to '{inputButton}' but the key was blacklisted.");
                return;
            }

            switch (inputButton)
            {
                case InputButton.Confirm:
                    this.currentConfig.Confirm = this.lastHitKey;
                    break;
                case InputButton.Back:
                    this.currentConfig.Back = this.lastHitKey;
                    break;
                case InputButton.Menu:
                    this.currentConfig.Menu = this.lastHitKey;
                    break;
                case InputButton.Special:
                    this.currentConfig.Special = this.lastHitKey;
                    break;
                case InputButton.Pause:
                    this.currentConfig.Pause = this.lastHitKey;
                    break;
                case InputButton.Select:
                    this.currentConfig.Select = this.lastHitKey;
                    break;
                default:
                    Debug.LogError("[InputManager:StateSetCurrentConfigButton] InputButton was null or invalid");
                    break;
            }

            states -= this.buttonEvent;
        }

        private void StateSetKeyboardDirection(InputDirection inputDirection)
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            if (IsKeyCodeBlacklisted(this.lastHitKey))
            {
                Debug.LogWarning($"[InputManager:StateSetCurrentKeyboardDirection] Tried to bind key '{this.lastHitKey}' to '{inputDirection}' but the key was blacklisted.");
                return;
            }

            switch (inputDirection)
            {
                case InputDirection.Up:
                    this.keyboardConfig.Up = this.lastHitKey;
                    break;
                case InputDirection.Left:
                    this.keyboardConfig.Left = this.lastHitKey;
                    break;
                case InputDirection.Down:
                    this.keyboardConfig.Down = this.lastHitKey;
                    break;
                case InputDirection.Right:
                    this.keyboardConfig.Right = this.lastHitKey;
                    break;
                default:
                    Debug.LogError("[InputManager:StateSetKeyboardDirection] InputDirection was null or invalid");
                    break;
            }

            states -= this.directionEvent;
        }

        private bool IsKeyCodeBlacklisted(KeyCode code)
        {
            return this.blacklistedCharacters.Contains(code);
        }
    }
}

