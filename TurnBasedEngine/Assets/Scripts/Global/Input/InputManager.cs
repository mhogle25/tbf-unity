using BF2D.Enums;
using UnityEngine;
using System;
using Newtonsoft.Json;

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
            [JsonIgnore] public KeyCode Attack { get { return this.attack; } set { this.attack = value; } }
            [JsonProperty] private protected KeyCode attack = KeyCode.U;
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
                    InputButton.Attack => this.attack,
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
            [JsonProperty] private string id = "keyboard_default";

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
            [JsonProperty] private string id = "gamepad_default";

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
                this.attack = KeyCode.JoystickButton2;
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

        [SerializeReference] private static ControlsConfigKeyboard defaultKeyboardConfig;
        [SerializeReference] private static ControlsConfigGamepad defaultGamepadConfig;
        [SerializeReference] private static ControlsConfigKeyboard keyboardConfig;
        [SerializeReference] private static ControlsConfigGamepad gamepadConfig;

        private static ControlsConfig currentConfig;
        private static KeyCode lastHitKey;

        private Action states;
        private Action buttonLambda;
        private Action directionLambda;

        public static bool Up { get { return GetDirection(InputDirection.Up); } }
        public static bool Left { get { return GetDirection(InputDirection.Left); } }
        public static bool Down { get { return GetDirection(InputDirection.Down); } }
        public static bool Right { get { return GetDirection(InputDirection.Right); } }
        public static bool Confirm { get { return GetButton(InputButton.Confirm); } }
        public static bool Back { get { return GetButton(InputButton.Back); } }
        public static bool Menu { get { return GetButton(InputButton.Menu); } }
        public static bool Attack { get { return GetButton(InputButton.Attack); } }
        public static bool Pause { get { return GetButton(InputButton.Pause); } }
        public static bool Select { get { return GetButton(InputButton.Select); } }
        public static bool ConfirmPress { get { return GetButtonPress(InputButton.Confirm); } }
        public static bool BackPress { get { return GetButtonPress(InputButton.Back); } }
        public static bool MenuPress { get { return GetButtonPress(InputButton.Menu); } }
        public static bool AttackPress { get { return GetButtonPress(InputButton.Attack); } }
        public static bool PausePress { get { return GetButtonPress(InputButton.Pause); } }
        public static bool SelectPress { get { return GetButtonPress(InputButton.Select); } }
        public static bool ConfirmRelease { get { return GetButtonRelease(InputButton.Confirm); } }
        public static bool BackRelease { get { return GetButtonRelease(InputButton.Back); } }
        public static bool MenuRelease { get { return GetButtonRelease(InputButton.Menu); } }
        public static bool AttackRelease { get { return GetButtonRelease(InputButton.Attack); } }
        public static bool PauseRelease { get { return GetButtonRelease(InputButton.Pause); } }
        public static bool SelectRelease { get { return GetButtonRelease(InputButton.Select); } }
        public static float HorizontalAxis { get { return GetAxis(Axis.Horizontal); } }
        public static float VerticalAxis { get { return GetAxis(Axis.Vertical); } }

        public static string KeyboardID { get { return InputManager.keyboardConfig.ID; } set { InputManager.keyboardConfig.ID = value; } }
        public static string GamepadID { get { return InputManager.gamepadConfig.ID; } set { InputManager.gamepadConfig.ID = value; } }

        public InputManager()
        {
            //Set up the default configs 
            InputManager.defaultKeyboardConfig = new ControlsConfigKeyboard();
            InputManager.defaultGamepadConfig = new ControlsConfigGamepad();

            //Set up the keyboard and gamepad configs
            InputManager.keyboardConfig = CloneKeyboardConfig(InputManager.defaultKeyboardConfig);
            InputManager.gamepadConfig = CloneGamepadConfig(InputManager.defaultGamepadConfig);

            //Initialize the current config
            InputManager.currentConfig = InputManager.keyboardConfig;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
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
                InputManager.lastHitKey = Event.current.keyCode;
            }
        }

        public static bool GetDirection(InputDirection inputDirection)
        {
            return InputManager.currentConfig.GetDirection(inputDirection);
        }

        public static bool GetButton(InputButton inputButton)
        {
            return InputManager.currentConfig.GetButton(inputButton);
        }

        public static bool GetButtonPress(InputButton inputButton)
        {
            return InputManager.currentConfig.GetButtonPress(inputButton);
        }

        public static bool GetButtonPress()
        {
            return InputManager.ConfirmPress || InputManager.BackPress || InputManager.MenuPress || InputManager.AttackPress || InputManager.PausePress || InputManager.SelectPress;
        }

        public static bool GetButtonRelease(InputButton inputButton)
        {
            return InputManager.currentConfig.GetButtonPress(inputButton);
        }

        public static bool GetButtonRelease()
        {
            return InputManager.ConfirmRelease || InputManager.BackRelease || InputManager.MenuRelease || InputManager.AttackRelease || InputManager.PauseRelease || InputManager.SelectRelease;
        }

        public static float GetAxis(Axis axis)
        {
            return InputManager.currentConfig.GetAxis(axis);
        }

        /// <summary>
        /// Returns the config of the keyboard to its default values
        /// </summary>
        public static void ResetConfig(InputController inputController)
        {
            switch (inputController)
            {
                case InputController.Keyboard:
                    InputManager.keyboardConfig = CloneKeyboardConfig(InputManager.defaultKeyboardConfig);
                    break;
                case InputController.Gamepad:
                    InputManager.gamepadConfig = CloneGamepadConfig(InputManager.defaultGamepadConfig);
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
        public static InputController GetCurrentControlType()
        {
            return InputManager.currentConfig.ControlType;
        }

        public static string SerializeConfig(InputController controllerType)
        {
            return controllerType switch
            {
                InputController.Keyboard => BF2D.Utilities.TextFile.SerializeObject<ControlsConfigKeyboard>(InputManager.keyboardConfig),
                InputController.Gamepad => BF2D.Utilities.TextFile.SerializeObject<ControlsConfigGamepad>(InputManager.gamepadConfig),
                _ => string.Empty,
            };
        }

        public static void DeserializeConfig(InputController controllerType, string json)
        {
            switch (controllerType)
            {
                case InputController.Keyboard:
                    InputManager.keyboardConfig = BF2D.Utilities.TextFile.DeserializeString<ControlsConfigKeyboard>(json);
                    break;
                case InputController.Gamepad:
                    InputManager.gamepadConfig = BF2D.Utilities.TextFile.DeserializeString<ControlsConfigGamepad>(json);
                    break;
                default:
                    Debug.LogError("[InputManager:DeserializeConfig] InputController was null or invalid");
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
        private static ControlsConfigKeyboard CloneKeyboardConfig(ControlsConfigKeyboard config)
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
                Attack = config.Attack,
                Pause = config.Pause,
                Select = config.Select
            };
        }

        /// <summary>
        /// Creates a copy of a Gamepad Config
        /// </summary>
        /// <param iconID="config">The Gamepad Config to be copied</param>
        /// <returns>The copy of the Gamepad Config</returns>
        private static ControlsConfigGamepad CloneGamepadConfig(ControlsConfigGamepad config)
        {
            return new ControlsConfigGamepad
            {
                Confirm = config.Confirm,
                Back = config.Back,
                Menu = config.Menu,
                Attack = config.Attack,
                Pause = config.Pause,
                Select = config.Select,
                JoystickThreshold = config.JoystickThreshold
            };
        }

        private static void ReloadCurrentConfig()
        {
            InputManager.currentConfig = InputManager.currentConfig.ControlType switch
            {
                InputController.Keyboard => InputManager.keyboardConfig,
                InputController.Gamepad => InputManager.gamepadConfig,
                _ => null,
            };

            if (InputManager.currentConfig is null)
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
                    InputManager.currentConfig = InputManager.gamepadConfig;
                }
            }
            else
            {
                if (GetCurrentControlType() == InputController.Gamepad)
                {
                    InputManager.currentConfig = InputManager.keyboardConfig;
                }
            }
        }

        private void StateSetCurrentConfigButton(InputButton inputButton)
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            switch (inputButton)
            {
                case InputButton.Confirm:
                    InputManager.currentConfig.Confirm = InputManager.lastHitKey;
                    break;
                case InputButton.Back:
                    InputManager.currentConfig.Back = InputManager.lastHitKey;
                    break;
                case InputButton.Menu:
                    InputManager.currentConfig.Menu = InputManager.lastHitKey;
                    break;
                case InputButton.Attack:
                    InputManager.currentConfig.Attack = InputManager.lastHitKey;
                    break;
                case InputButton.Pause:
                    InputManager.currentConfig.Pause = InputManager.lastHitKey;
                    break;
                case InputButton.Select:
                    InputManager.currentConfig.Select = InputManager.lastHitKey;
                    break;
                default:
                    Debug.LogError("[InputManager:StateSetCurrentConfigButton] InputButton was null or invalid");
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

            switch (inputDirection)
            {
                case InputDirection.Up:
                    InputManager.keyboardConfig.Up = InputManager.lastHitKey;
                    break;
                case InputDirection.Left:
                    InputManager.keyboardConfig.Left = InputManager.lastHitKey;
                    break;
                case InputDirection.Down:
                    InputManager.keyboardConfig.Down = InputManager.lastHitKey;
                    break;
                case InputDirection.Right:
                    InputManager.keyboardConfig.Right = InputManager.lastHitKey;
                    break;
                default:
                    Debug.LogError("[InputManager:StateSetKeyboardDirection] InputDirection was null or invalid");
                    break;

            }

            states -= directionLambda;
        }
    }
}

