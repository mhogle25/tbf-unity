using BF2D.Enums;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BF2D
{
    public class InputManager : MonoBehaviour
    {

        [Serializable]
        public abstract class ControlsConfig
        {
            [JsonIgnore] public abstract string ID { get; set; }

            [JsonIgnore] public abstract InputController ControlType { get; }
            [JsonIgnore] public KeyCode Confirm { get { return this.confirm; } set { this.confirm = value; } }
            [JsonProperty] private protected KeyCode confirm = KeyCode.P;
            [JsonIgnore] public KeyCode Back { get { return this.back; } set { this.back = value; } }
            [JsonProperty] private protected KeyCode back = KeyCode.O;
            [JsonIgnore] public KeyCode Menu { get { return this.menu; } set { this.menu = value; } }
            [JsonProperty] private protected KeyCode menu = KeyCode.I;
            [JsonIgnore] public KeyCode Special { get { return this.special; } set { this.special = value; } }
            [JsonProperty] private protected KeyCode special = KeyCode.U;
            [JsonIgnore] public KeyCode Pause { get { return this.pause; } set { this.pause = value; } }
            [JsonProperty] private protected KeyCode pause = KeyCode.Escape;
            [JsonIgnore] public KeyCode Select { get { return this.select; } set { this.select = value; } }
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
            [JsonIgnore] public override string ID { get { return this.id; } set { this.id = value; } }
            [JsonProperty] private string id = "default";

            [JsonIgnore] public override InputController ControlType { get { return ControlsConfigKeyboard.controlType; } }
            private const InputController controlType = InputController.Keyboard;
            [JsonIgnore] public KeyCode Up { get { return this.up; } set { this.up = value; } }
            [JsonProperty] private KeyCode up = KeyCode.W;
            [JsonIgnore] public KeyCode Left { get { return this.left; } set { this.left = value; } }
            [JsonProperty] private KeyCode left = KeyCode.A;
            [JsonIgnore] public KeyCode Down { get { return this.down; } set { this.down = value; } }
            [JsonProperty] private KeyCode down = KeyCode.S;
            [JsonIgnore] public KeyCode Right { get { return this.right; } set { this.right = value; } }
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
                    tempHor *= (Mathf.Sqrt(2) / 2);
                    tempVer *= (Mathf.Sqrt(2) / 2);
                }

                return axis switch
                {
                    Axis.Horizontal => tempHor,
                    Axis.Vertical => tempVer,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigKeyboard:GetAxis] Axis was null or invalid"),
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

            [JsonIgnore] public override string ID { get { return this.id; } set { this.id = value; } }
            [JsonProperty] private string id = "default";

            [JsonIgnore] public override InputController ControlType { get { return ControlsConfigGamepad.controlType; } }
            private const InputController controlType = InputController.Gamepad;
            [JsonIgnore] public float JoystickThreshold { get { return this.joystickThreshold; } set { this.joystickThreshold = value; } }
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
                    Axis.Horizontal => HORIZONTAL_AXIS,
                    Axis.Vertical => VERTICAL_AXIS,
                    _ => throw new ArgumentException("[InputManager:ControlsConfigGamepad:GetAxisKey] Axis was null or invalid"),
                };
            }

            private string GetDAxisKey(Axis axis)
            {
                return axis switch
                {
                    Axis.Horizontal => D_HORIZONTAL_AXIS,
                    Axis.Vertical => D_VERTICAL_AXIS,
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
        }

        [SerializeField] private List<KeyCode> blacklistedCharacters = new();

        public bool InputEnabled { get { return this.inputEnabled; } set { this.inputEnabled = value; } }
        private bool inputEnabled = true;

        private readonly ControlsConfigKeyboard defaultKeyboardConfig;
        private readonly ControlsConfigGamepad defaultGamepadConfig;
        private ControlsConfigKeyboard keyboardConfig;
        private ControlsConfigGamepad gamepadConfig;

        private ControlsConfig currentConfig;
        private KeyCode lastHitKey;

        private Action states;
        private Action buttonLambda;
        private Action directionLambda;

        public bool Up { get { return GetDirection(InputDirection.Up); } }
        public bool Left { get { return GetDirection(InputDirection.Left); } }
        public bool Down { get { return GetDirection(InputDirection.Down); } }
        public bool Right { get { return GetDirection(InputDirection.Right); } }
        public bool Confirm { get { return GetButton(InputButton.Confirm); } }
        public bool Back { get { return GetButton(InputButton.Back); } }
        public bool Menu { get { return GetButton(InputButton.Menu); } }
        public bool Special { get { return GetButton(InputButton.Special); } }
        public bool Pause { get { return GetButton(InputButton.Pause); } }
        public bool Select { get { return GetButton(InputButton.Select); } }
        public bool ConfirmPress { get { return GetButtonPress(InputButton.Confirm); } }
        public bool BackPress { get { return GetButtonPress(InputButton.Back); } }
        public bool MenuPress { get { return GetButtonPress(InputButton.Menu); } }
        public bool SpecialPress { get { return GetButtonPress(InputButton.Special); } }
        public bool PausePress { get { return GetButtonPress(InputButton.Pause); } }
        public bool SelectPress { get { return GetButtonPress(InputButton.Select); } }
        public bool ConfirmRelease { get { return GetButtonRelease(InputButton.Confirm); } }
        public bool BackRelease { get { return GetButtonRelease(InputButton.Back); } }
        public bool MenuRelease { get { return GetButtonRelease(InputButton.Menu); } }
        public bool SpecialRelease { get { return GetButtonRelease(InputButton.Special); } }
        public bool PauseRelease { get { return GetButtonRelease(InputButton.Pause); } }
        public bool SelectRelease { get { return GetButtonRelease(InputButton.Select); } }
        public float HorizontalAxis { get { return GetAxis(Axis.Horizontal); } }
        public float VerticalAxis { get { return GetAxis(Axis.Vertical); } }

        public string KeyboardID { get { return this.keyboardConfig.ID; } set { this.keyboardConfig.ID = value; } }
        public string GamepadID { get { return this.gamepadConfig.ID; } set { this.gamepadConfig.ID = value; } }

        public static InputManager Instance { get { return InputManager.instance; } }
        private static InputManager instance = null;

        public InputManager()
        {
            //Set up the default configs 
            this.defaultKeyboardConfig = new ControlsConfigKeyboard();
            this.defaultGamepadConfig = new ControlsConfigGamepad();

            //Set up the keyboard and gamepad configs
            this.keyboardConfig = CloneKeyboardConfig(this.defaultKeyboardConfig);
            this.gamepadConfig = CloneGamepadConfig(this.defaultGamepadConfig);

            //Initialize the current config
            this.currentConfig = this.keyboardConfig;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            //Setup of Monobehaviour Singleton
            if (InputManager.instance != this && InputManager.instance != null)
            {
                Destroy(InputManager.instance.gameObject);
            }

            InputManager.instance = this;

            states += StateGamepadConnected;
        }

        private void Update()
        {
            states?.Invoke();
        }

        private void OnGUI()
        {
            if (Input.anyKeyDown)
            {
                this.lastHitKey = Event.current.keyCode;
            }
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
                    this.keyboardConfig = CloneKeyboardConfig(this.defaultKeyboardConfig);
                    break;
                case InputController.Gamepad:
                    this.gamepadConfig = CloneGamepadConfig(this.defaultGamepadConfig);
                    break;
                default:
                    Terminal.IO.LogError("[InputManager:ResetConfig] InputController was null or invalid");
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
                InputController.Keyboard => BF2D.Utilities.JSON.SerializeObject<ControlsConfigKeyboard>(this.keyboardConfig),
                InputController.Gamepad => BF2D.Utilities.JSON.SerializeObject<ControlsConfigGamepad>(this.gamepadConfig),
                _ => string.Empty,
            };
        }

        public void DeserializeConfig(InputController controllerType, string json)
        {
            switch (controllerType)
            {
                case InputController.Keyboard:
                    this.keyboardConfig = BF2D.Utilities.JSON.DeserializeString<ControlsConfigKeyboard>(json);
                    break;
                case InputController.Gamepad:
                    this.gamepadConfig = BF2D.Utilities.JSON.DeserializeString<ControlsConfigGamepad>(json);
                    break;
                default:
                    Terminal.IO.LogError("[InputManager:DeserializeConfig] InputController was null or invalid");
                    break;
            }

            ReloadCurrentConfig();
        }

        public void SetCurrentConfigButton(InputButton inputButton)
        {
            buttonLambda = () =>
            {
                StateSetCurrentConfigButton(inputButton);
            };
            states += buttonLambda;
        }

        public void SetKeyboardDirection(InputDirection inputDirection)
        {
            directionLambda = () =>
            {
                StateSetKeyboardDirection(inputDirection);
            };
            states += directionLambda;
        }

        /// <summary>
        /// Creates a copy of a Keyboard Config
        /// </summary>
        /// <param iconID="config">The Keyboard Config to be copied</param>
        /// <returns>The copy of the Keyboard Config</returns>
        private ControlsConfigKeyboard CloneKeyboardConfig(ControlsConfigKeyboard config)
        {
            return new ControlsConfigKeyboard
            {
                Up = config.Up,
                Left = config.Left,
                Down = config.Down,
                Right = config.Right,
                Confirm = config.Confirm,
                Back = config.Back,
                Menu = config.Menu,
                Special = config.Special,
                Pause = config.Pause,
                Select = config.Select
            };
        }

        /// <summary>
        /// Creates a copy of a Gamepad Config
        /// </summary>
        /// <param iconID="config">The Gamepad Config to be copied</param>
        /// <returns>The copy of the Gamepad Config</returns>
        private ControlsConfigGamepad CloneGamepadConfig(ControlsConfigGamepad config)
        {
            return new ControlsConfigGamepad
            {
                Confirm = config.Confirm,
                Back = config.Back,
                Menu = config.Menu,
                Special = config.Special,
                Pause = config.Pause,
                Select = config.Select,
                JoystickThreshold = config.JoystickThreshold
            };
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
                Terminal.IO.LogError("[InputManager:ReloadCurrentConfig] InputController was null or invalid");
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
                Terminal.IO.LogWarning($"[InputManager:StateSetCurrentConfigButton] Tried to bind key '{this.lastHitKey}' to '{inputButton}' but the key was blacklisted.");
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
                    Terminal.IO.LogError("[InputManager:StateSetCurrentConfigButton] InputButton was null or invalid");
                    break;
            }

            states -= buttonLambda;
        }

        private void StateSetKeyboardDirection(InputDirection inputDirection)
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            if (IsKeyCodeBlacklisted(this.lastHitKey))
            {
                Terminal.IO.LogWarning($"[InputManager:StateSetCurrentKeyboardDirection] Tried to bind key '{this.lastHitKey}' to '{inputDirection}' but the key was blacklisted.");
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
                    Terminal.IO.LogError("[InputManager:StateSetKeyboardDirection] InputDirection was null or invalid");
                    break;
            }

            states -= directionLambda;
        }

        private bool IsKeyCodeBlacklisted(KeyCode code)
        {
            return this.blacklistedCharacters.Contains(code);
        }
    }
}

