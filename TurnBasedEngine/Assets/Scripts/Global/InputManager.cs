using System;
using UnityEngine;
using UnityEngine.Animations;

namespace BF2D {

    public class InputManager : MonoBehaviour {
        #region Key
        public static bool Up { get { return InputManager.up; } }
        private static bool up = false;
        public static bool Left { get { return InputManager.left; } }
        private static bool left = false;
        public static bool Down { get { return InputManager.down; } }
        private static bool down = false;
        public static bool Right { get { return InputManager.right; } }
        private static bool right = false;
        public static bool Confirm { get { return InputManager.confirm; } }
        private static bool confirm = false;
        public static bool Back { get { return InputManager.back; } }
        private static bool back = false;
        public static bool Menu { get { return InputManager.menu; } }
        private static bool menu = false;
        public static bool Attack { get { return InputManager.attack; } }
        private static bool attack = false;
        public static bool Pause { get { return InputManager.pause; } }
        private static bool pause = false;
        public static bool Select { get { return InputManager.select; } }
        private static bool select = false;

        public static bool Key(InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.Up:
                    return InputManager.up;
                case InputKey.Left:
                    return InputManager.left;
                case InputKey.Down:
                    return InputManager.down;
                case InputKey.Right:
                    return InputManager.right;
                case InputKey.Confirm:
                    return InputManager.confirm;
                case InputKey.Back:
                    return InputManager.back;
                case InputKey.Attack:
                    return InputManager.attack;
                case InputKey.Menu:
                    return InputManager.menu;
                case InputKey.Pause:
                    return InputManager.pause;
                case InputKey.Select:
                    return InputManager.select;
                default:
                    throw new ArgumentException("Input Key was null or invalid");
            }
        }
        #endregion

        #region Key Press
        public static bool UpPress { get { return InputManager.upPress; } }
        private static bool upPress = false;
        public static bool LeftPress { get { return InputManager.leftPress; } }
        private static bool leftPress = false;
        public static bool DownPress { get { return InputManager.downPress; } }
        private static bool downPress = false;
        public static bool RightPress { get { return InputManager.rightPress; } }
        private static bool rightPress = false;
        public static bool ConfirmPress { get { return InputManager.confirmPress; } }
        private static bool confirmPress = false;
        public static bool BackPress { get { return InputManager.backPress; } }
        private static bool backPress = false;
        public static bool MenuPress { get { return InputManager.menuPress; } }
        private static bool menuPress = false;
        public static bool AttackPress { get { return InputManager.attackPress; } }
        private static bool attackPress = false;
        public static bool PausePress { get { return InputManager.pausePress; } }
        private static bool pausePress = false;
        public static bool SelectPress { get { return InputManager.selectPress; } }
        private static bool selectPress = false;

        public static bool KeyPress(InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.Up:
                    return InputManager.upPress;
                case InputKey.Left:
                    return InputManager.leftPress;
                case InputKey.Down:
                    return InputManager.downPress;
                case InputKey.Right:
                    return InputManager.rightPress;
                case InputKey.Confirm:
                    return InputManager.confirmPress;
                case InputKey.Back:
                    return InputManager.backPress;
                case InputKey.Attack:
                    return InputManager.attackPress;
                case InputKey.Menu:
                    return InputManager.menuPress;
                case InputKey.Pause:
                    return InputManager.pausePress;
                case InputKey.Select:
                    return InputManager.selectPress;
                default:
                    throw new ArgumentException("Input Key was null or invalid");
            }
        }
        #endregion

        #region Key Release
        public static bool UpRelease { get { return InputManager.upRelease; } }
        private static bool upRelease = false;
        public static bool LeftRelease { get { return InputManager.leftRelease; } }
        private static bool leftRelease = false;
        public static bool DownRelease { get { return InputManager.downRelease; } }
        private static bool downRelease = false;
        public static bool RightRelease { get { return InputManager.rightRelease; } }
        private static bool rightRelease = false;
        public static bool ConfirmRelease { get { return InputManager.confirmRelease; } }
        private static bool confirmRelease = false;
        public static bool BackRelease { get { return InputManager.backRelease; } }
        private static bool backRelease = false;
        public static bool MenuRelease { get { return InputManager.menuRelease; } }
        private static bool menuRelease = false;
        public static bool AttackRelease { get { return InputManager.attackRelease; } }
        private static bool attackRelease = false;
        public static bool PauseRelease { get { return InputManager.pauseRelease; } }
        private static bool pauseRelease = false;
        public static bool SelectRelease { get { return InputManager.selectRelease; } }
        private static bool selectRelease = false;

        public static bool KeyRelease(InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.Up:
                    return InputManager.upRelease;
                case InputKey.Left:
                    return InputManager.leftRelease;
                case InputKey.Down:
                    return InputManager.downRelease;
                case InputKey.Right:
                    return InputManager.rightRelease;
                case InputKey.Confirm:
                    return InputManager.confirmRelease;
                case InputKey.Back:
                    return InputManager.backRelease;
                case InputKey.Attack:
                    return InputManager.attackRelease;
                case InputKey.Menu:
                    return InputManager.menuRelease;
                case InputKey.Pause:
                    return InputManager.pauseRelease;
                case InputKey.Select:
                    return InputManager.selectRelease;
                default:
                    throw new ArgumentException("Input Key was null or invalid");
            }
        }
        #endregion

        #region Misc
        public static bool IsGamepadConnected { get { return _isGamepadConnected; } }
        private static bool _isGamepadConnected = false;
        public static float HorizontalAxis { get { return _horizontalAxis; } }
        private static float _horizontalAxis = 0;
        public static float VerticalAxis { get { return _verticalAxis; } }
        private static float _verticalAxis = 0;
        #endregion

        [SerializeField] private float joystickThreshold = 0.5f;

        private Action inputListener;

        private bool gamepadLeftPressFlag = true;
        private bool gamepadRightPressFlag = true;
        private bool gamepadUpPressFlag = true;
        private bool gamepadDownPressFlag = true;
        private bool gamepadLeftReleaseFlag = true;
        private bool gamepadRightReleaseFlag = true;
        private bool gamepadUpReleaseFlag = true;
        private bool gamepadDownReleaseFlag = true;

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            this.inputListener += Keyboard;
            this.inputListener += GamepadConnected;
        }

        private void LateUpdate()
        {
            //Call input listener methods
            this.inputListener();
        }

        private void Gamepad() {
            //Set Key Flags
            InputManager.up = Input.GetAxis("Vertical") > this.joystickThreshold;
            InputManager.left = Input.GetAxis("Horizontal") < -this.joystickThreshold;
            InputManager.down = Input.GetAxis("Vertical") < -this.joystickThreshold;
            InputManager.right = Input.GetAxis("Horizontal") > this.joystickThreshold;
            InputManager.confirm = Input.GetKey(ControlsConfig.GamepadConfirm);
            InputManager.back = Input.GetKey(ControlsConfig.GamepadBack);
            InputManager.menu = Input.GetKey(ControlsConfig.GamepadMenu);
            InputManager.attack = Input.GetKey(ControlsConfig.GamepadAttack);
            InputManager.pause = Input.GetKey(ControlsConfig.GamepadPause);
            InputManager.select = Input.GetKey(ControlsConfig.GamepadSelect);

            //Set Key Press Flags
            if (Input.GetAxis("Vertical") > this.joystickThreshold) {
                if (!InputManager.upPress && this.gamepadUpPressFlag) {
                    InputManager.upPress = true;
                } else if (InputManager.upPress && this.gamepadUpPressFlag) {
                    InputManager.upPress = false;
                    this.gamepadUpPressFlag = false;
                }
            } else {
                if (!InputManager.upPress && !this.gamepadUpPressFlag) {
                    this.gamepadUpPressFlag = true;
                }
            }

            if (Input.GetAxis("Horizontal") < -this.joystickThreshold) {
                if (!InputManager.leftPress && this.gamepadLeftPressFlag) {
                    InputManager.leftPress = true;
                } else if (InputManager.leftPress && this.gamepadLeftPressFlag) {
                    InputManager.leftPress = false;
                    this.gamepadLeftPressFlag = false;
                }
            } else {
                if (!InputManager.leftPress && !this.gamepadLeftPressFlag) {
                    this.gamepadLeftPressFlag = true;
                }
            }

            if (Input.GetAxis("Vertical") < -this.joystickThreshold) {
                if (!InputManager.downPress && this.gamepadDownPressFlag) {
                    InputManager.downPress = true;
                } else if (InputManager.downPress && this.gamepadDownPressFlag) {
                    InputManager.downPress = false;
                    this.gamepadDownPressFlag = false;
                }
            } else {
                if (!InputManager.downPress && !this.gamepadDownPressFlag) {
                    this.gamepadDownPressFlag = true;
                }
            }

            if (Input.GetAxis("Horizontal") > this.joystickThreshold) {
                if (!InputManager.rightPress && this.gamepadRightPressFlag) {
                    InputManager.rightPress = true;
                } else if (InputManager.rightPress && this.gamepadRightPressFlag) {
                    InputManager.rightPress = false;
                    this.gamepadRightPressFlag = false;
                }
            } else {
                if (!InputManager.rightPress && !this.gamepadRightPressFlag) {
                    this.gamepadRightPressFlag = true;
                }
            }

            InputManager.confirmPress = Input.GetKeyDown(ControlsConfig.GamepadConfirm);
            InputManager.backPress = Input.GetKeyDown(ControlsConfig.GamepadBack);
            InputManager.menuPress = Input.GetKeyDown(ControlsConfig.GamepadMenu);
            InputManager.attackPress = Input.GetKeyDown(ControlsConfig.GamepadAttack);
            InputManager.pausePress = Input.GetKeyDown(ControlsConfig.GamepadPause);
            InputManager.selectPress = Input.GetKeyDown(ControlsConfig.GamepadSelect);

            //Set Key Release Flags
            if (Input.GetAxis("Vertical") < this.joystickThreshold) {
                if (!InputManager.upRelease && this.gamepadUpReleaseFlag) {
                    InputManager.upRelease = true;
                } else if (InputManager.upRelease && this.gamepadUpReleaseFlag) {
                    InputManager.upRelease = false;
                    this.gamepadUpReleaseFlag = false;
                }
            } else {
                if (!InputManager.upRelease && !this.gamepadUpReleaseFlag) {
                    this.gamepadUpReleaseFlag = true;
                }
            }

            if (Input.GetAxis("Horizontal") > -this.joystickThreshold) {
                if (!InputManager.leftRelease && this.gamepadLeftReleaseFlag) {
                    InputManager.leftRelease = true;
                } else if (InputManager.leftRelease && this.gamepadLeftReleaseFlag) {
                    InputManager.leftRelease = false;
                    this.gamepadLeftReleaseFlag = false;
                }
            } else {
                if (!InputManager.leftRelease && !this.gamepadLeftReleaseFlag) {
                    this.gamepadLeftReleaseFlag = true;
                }
            }

            if (Input.GetAxis("Vertical") > -this.joystickThreshold) {
                if (!InputManager.downRelease && this.gamepadDownReleaseFlag) {
                    InputManager.downRelease = true;
                } else if (InputManager.downRelease && this.gamepadDownReleaseFlag) {
                    InputManager.downRelease = false;
                    this.gamepadDownReleaseFlag = false;
                }
            } else {
                if (!InputManager.downRelease && !this.gamepadDownReleaseFlag) {
                    this.gamepadDownReleaseFlag = true;
                }
            }

            if (Input.GetAxis("Horizontal") < this.joystickThreshold) {
                if (!InputManager.rightRelease && this.gamepadRightReleaseFlag) {
                    InputManager.rightRelease = true;
                } else if (InputManager.rightRelease && this.gamepadRightReleaseFlag) {
                    InputManager.rightRelease = false;
                    this.gamepadRightReleaseFlag = false;
                }
            } else {
                if (!InputManager.rightRelease && !this.gamepadRightReleaseFlag) {
                    this.gamepadRightReleaseFlag = true;
                }
            }

            InputManager.confirmRelease = Input.GetKeyUp(ControlsConfig.GamepadConfirm);
            InputManager.backRelease = Input.GetKeyUp(ControlsConfig.GamepadBack);
            InputManager.menuRelease = Input.GetKeyUp(ControlsConfig.GamepadMenu);
            InputManager.attackRelease = Input.GetKeyUp(ControlsConfig.GamepadAttack);
            InputManager.pauseRelease = Input.GetKeyUp(ControlsConfig.GamepadPause);
            InputManager.selectRelease = Input.GetKeyUp(ControlsConfig.GamepadSelect);

            //Set Horizontal and Vertical Axis Values
            _horizontalAxis = Input.GetAxis("Horizontal");
            _verticalAxis = Input.GetAxis("Vertical");

            //Set Directional Press flags
            if (_horizontalAxis > this.joystickThreshold) {
                InputManager.right = true;
            } else if (_horizontalAxis < -this.joystickThreshold) {
                InputManager.left = true;
            } else {
                InputManager.left = false;
                InputManager.right = false;
            }

            if (_verticalAxis > this.joystickThreshold) {
                InputManager.up = true;
            } else if (_verticalAxis < -this.joystickThreshold) {
                InputManager.down = true;
            } else {
                InputManager.up = false;
                InputManager.down = false;
            }
        }

        private void Keyboard() {
            //Set key flags
            InputManager.up = Input.GetKey(ControlsConfig.KeyboardUp);
            InputManager.left = Input.GetKey(ControlsConfig.KeyboardLeft);
            InputManager.down = Input.GetKey(ControlsConfig.KeyboardDown);
            InputManager.right = Input.GetKey(ControlsConfig.KeyboardRight);
            InputManager.confirm = Input.GetKey(ControlsConfig.KeyboardConfirm);
            InputManager.back = Input.GetKey(ControlsConfig.KeyboardBack);
            InputManager.menu = Input.GetKey(ControlsConfig.KeyboardMenu);
            InputManager.attack = Input.GetKey(ControlsConfig.KeyboardAttack);
            InputManager.pause = Input.GetKey(ControlsConfig.KeyboardPause);
            InputManager.select = Input.GetKey(ControlsConfig.KeyboardSelect);

            //Set key press flags
            InputManager.upPress = Input.GetKeyDown(ControlsConfig.KeyboardUp);
            InputManager.leftPress = Input.GetKeyDown(ControlsConfig.KeyboardLeft);
            InputManager.downPress = Input.GetKeyDown(ControlsConfig.KeyboardDown);
            InputManager.rightPress = Input.GetKeyDown(ControlsConfig.KeyboardRight);
            InputManager.confirmPress = Input.GetKeyDown(ControlsConfig.KeyboardConfirm);
            InputManager.backPress = Input.GetKeyDown(ControlsConfig.KeyboardBack);
            InputManager.menuPress = Input.GetKeyDown(ControlsConfig.KeyboardMenu);
            InputManager.attackPress = Input.GetKeyDown(ControlsConfig.KeyboardAttack);
            InputManager.pausePress = Input.GetKeyDown(ControlsConfig.KeyboardPause);
            InputManager.selectPress = Input.GetKeyDown(ControlsConfig.KeyboardSelect);

            //Set key release flags
            InputManager.upRelease = Input.GetKeyUp(ControlsConfig.KeyboardUp);
            InputManager.leftRelease = Input.GetKeyUp(ControlsConfig.KeyboardLeft);
            InputManager.downRelease = Input.GetKeyUp(ControlsConfig.KeyboardDown);
            InputManager.rightRelease = Input.GetKeyUp(ControlsConfig.KeyboardRight);
            InputManager.confirmRelease = Input.GetKeyUp(ControlsConfig.KeyboardConfirm);
            InputManager.backRelease = Input.GetKeyUp(ControlsConfig.KeyboardBack);
            InputManager.menuRelease = Input.GetKeyUp(ControlsConfig.KeyboardMenu);
            InputManager.attackRelease = Input.GetKeyUp(ControlsConfig.KeyboardAttack);
            InputManager.pauseRelease = Input.GetKeyUp(ControlsConfig.KeyboardPause);
            InputManager.selectRelease = Input.GetKeyUp(ControlsConfig.KeyboardSelect);

            //Set Horizontal and Vertical Axis Values
            float tempHor;
            float tempVer;
            if (InputManager.left && InputManager.right) {
                tempHor = 0;
            } else if (InputManager.left) {
                tempHor = -1;
            } else if (InputManager.right) {
                tempHor = 1;
            } else {
                tempHor = 0;
            }

            if (InputManager.up && InputManager.down) {
                tempVer = 0;
            } else if (InputManager.down) {
                tempVer = -1;
            } else if (InputManager.up) {
                tempVer = 1;
            } else {
                tempVer = 0;
            }

            //Calculate distances for diagonals (unit circle shit)
            if (tempVer != 0 && tempHor != 0) {
                tempHor *= (Mathf.Sqrt(2) / 2);
                tempVer *= (Mathf.Sqrt(2) / 2);
            }

            _horizontalAxis = tempHor;
            _verticalAxis = tempVer;
        }

        private void GamepadConnected() {
            //Check if gamepad is connected
            string[] gamepadNames = Input.GetJoystickNames();
            if (gamepadNames.Length > 0) {
                if (gamepadNames[0] != string.Empty) {
                    if (!_isGamepadConnected) {
                        _isGamepadConnected = true;
                        this.inputListener += Gamepad;
                        this.inputListener -= Keyboard;
                    }
                } else {
                    if (_isGamepadConnected) {
                        _isGamepadConnected = false;
                        this.inputListener -= Gamepad;
                        this.inputListener += Keyboard;
                    }
                }
            }
        }
    }
}

