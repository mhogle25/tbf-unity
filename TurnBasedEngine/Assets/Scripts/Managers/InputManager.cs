using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    #region KEY
    public static bool Up { get { return _up; } }
    private static bool _up = false;
    public static bool Left { get { return _left; } }
    private static bool _left = false;
    public static bool Down { get { return _down; } }
    private static bool _down = false;
    public static bool Right { get { return _right; } }
    private static bool _right = false;
    public static bool Confirm { get { return _confirm; } }
    private static bool _confirm = false;
    public static bool Back { get { return _back; } }
    private static bool _back = false;
    public static bool Menu { get { return _menu; } }
    private static bool _menu = false;
    public static bool Attack { get { return _attack; } }
    private static bool _attack = false;
    public static bool Pause { get { return _pause; } }
    private static bool _pause = false;
    public static bool Select { get { return _select; } }
    private static bool _select = false;
    #endregion

    #region KEY_PRESS
    public static bool UpPress { get { return _upPress; } }
    private static bool _upPress = false;
    public static bool LeftPress { get { return _leftPress; } }
    private static bool _leftPress = false;
    public static bool DownPress { get { return _downPress; } }
    private static bool _downPress = false;
    public static bool RightPress { get { return _rightPress; } }
    private static bool _rightPress = false;
    public static bool ConfirmPress { get { return _confirmPress; } }
    private static bool _confirmPress = false;
    public static bool BackPress { get { return _backPress; } }
    private static bool _backPress = false;
    public static bool MenuPress { get { return _menuPress; } }
    private static bool _menuPress = false;
    public static bool AttackPress { get { return _attackPress; } }
    private static bool _attackPress = false;
    public static bool PausePress { get { return _pausePress; } }
    private static bool _pausePress = false;
    public static bool SelectPress { get { return _selectPress; } }
    private static bool _selectPress = false;
    #endregion

    #region KEY_RELEASE
    public static bool UpRelease { get { return _upRelease; } }
    private static bool _upRelease = false;
    public static bool LeftRelease { get { return _leftRelease; } }
    private static bool _leftRelease = false;
    public static bool DownRelease { get { return _downRelease; } }
    private static bool _downRelease = false;
    public static bool RightRelease { get { return _rightRelease; } }
    private static bool _rightRelease = false;
    public static bool ConfirmRelease { get { return _confirmRelease; } }
    private static bool _confirmRelease = false;
    public static bool BackRelease { get { return _backRelease; } }
    private static bool _backRelease = false;
    public static bool MenuRelease { get { return _menuRelease; } }
    private static bool _menuRelease = false;
    public static bool AttackRelease { get { return _attackRelease; } }
    private static bool _attackRelease = false;
    public static bool PauseRelease { get { return _pauseRelease; } }
    private static bool _pauseRelease = false;
    public static bool SelectRelease { get { return _selectRelease; } }
    private static bool _selectRelease = false;
    #endregion

    #region MISC
    public static bool IsGamepadConnected { get { return _isGamepadConnected; } }
    private static bool _isGamepadConnected = false;
    public static float HorizontalAxis { get { return _horizontalAxis; } }
    private static float _horizontalAxis = 0;
    public static float VerticalAxis { get { return _verticalAxis; } }
    private static float _verticalAxis = 0;
    #endregion

    private Action inputListener;
    private float joystickThreshold = 0.5f;

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
        inputListener += Keyboard;
        inputListener += GamepadConnected;
    }

    private void Update() {
        //Call input listener methods
        inputListener();
    }

    private void Gamepad() {
        //Set Key Flags
        _up = Input.GetAxis("Vertical") > joystickThreshold;
        _left = Input.GetAxis("Horizontal") < -joystickThreshold;
        _down = Input.GetAxis("Vertical") < -joystickThreshold;
        _right = Input.GetAxis("Horizontal") > joystickThreshold;
        _confirm = Input.GetKey(ControlsConfig.GamepadConfirm);
        _back = Input.GetKey(ControlsConfig.GamepadBack);
        _menu = Input.GetKey(ControlsConfig.GamepadMenu);
        _attack = Input.GetKey(ControlsConfig.GamepadAttack);
        _pause = Input.GetKey(ControlsConfig.GamepadPause);
        _select = Input.GetKey(ControlsConfig.GamepadSelect);

        //Set Key Press Flags
        if (Input.GetAxis("Vertical") > joystickThreshold) {
            if (!_upPress && gamepadUpPressFlag) {
                _upPress = true;
            } else if (_upPress && gamepadUpPressFlag) {
                _upPress = false;
                gamepadUpPressFlag = false;
            }
        } else {
            if (!_upPress && !gamepadUpPressFlag) {
                gamepadUpPressFlag = true;
            }
        }

        if (Input.GetAxis("Horizontal") < -joystickThreshold) {
            if (!_leftPress && gamepadLeftPressFlag) {
                _leftPress = true;
            } else if (_leftPress && gamepadLeftPressFlag) {
                _leftPress = false;
                gamepadLeftPressFlag = false;
            }
        } else {
            if (!_leftPress && !gamepadLeftPressFlag) {
                gamepadLeftPressFlag = true;
            }
        }

        if (Input.GetAxis("Vertical") < -joystickThreshold) {
            if (!_downPress && gamepadDownPressFlag) {
                _downPress = true;
            } else if (_downPress && gamepadDownPressFlag) {
                _downPress = false;
                gamepadDownPressFlag = false;
            }
        } else {
            if (!_downPress && !gamepadDownPressFlag) {
                gamepadDownPressFlag = true;
            }
        }

        if (Input.GetAxis("Horizontal") > joystickThreshold) {
            if (!_rightPress && gamepadRightPressFlag) {
                _rightPress = true;
            } else if (_rightPress && gamepadRightPressFlag) {
                _rightPress = false;
                gamepadRightPressFlag = false;
            }
        } else {
            if (!_rightPress && !gamepadRightPressFlag) {
                gamepadRightPressFlag = true;
            }
        }

        _confirmPress = Input.GetKeyDown(ControlsConfig.GamepadConfirm);
        _backPress = Input.GetKeyDown(ControlsConfig.GamepadBack);
        _menuPress = Input.GetKeyDown(ControlsConfig.GamepadMenu);
        _attackPress = Input.GetKeyDown(ControlsConfig.GamepadAttack);
        _pausePress = Input.GetKeyDown(ControlsConfig.GamepadPause);
        _selectPress = Input.GetKeyDown(ControlsConfig.GamepadSelect);

        //Set Key Release Flags
        if (Input.GetAxis("Vertical") < joystickThreshold) {
            if (!_upRelease && gamepadUpReleaseFlag) {
                _upRelease = true;
            } else if (_upRelease && gamepadUpReleaseFlag) {
                _upRelease = false;
                gamepadUpReleaseFlag = false;
            }
        } else {
            if (!_upRelease && !gamepadUpReleaseFlag) {
                gamepadUpReleaseFlag = true;
            }
        }

        if (Input.GetAxis("Horizontal") > -joystickThreshold) {
            if (!_leftRelease && gamepadLeftReleaseFlag) {
                _leftRelease = true;
            } else if (_leftRelease && gamepadLeftReleaseFlag) {
                _leftRelease = false;
                gamepadLeftReleaseFlag = false;
            }
        } else {
            if (!_leftRelease && !gamepadLeftReleaseFlag) {
                gamepadLeftReleaseFlag = true;
            }
        }

        if (Input.GetAxis("Vertical") > -joystickThreshold) {
            if (!_downRelease && gamepadDownReleaseFlag) {
                _downRelease = true;
            } else if (_downRelease && gamepadDownReleaseFlag) {
                _downRelease = false;
                gamepadDownReleaseFlag = false;
            }
        } else {
            if (!_downRelease && !gamepadDownReleaseFlag) {
                gamepadDownReleaseFlag = true;
            }
        }

        if (Input.GetAxis("Horizontal") < joystickThreshold) {
            if (!_rightRelease && gamepadRightReleaseFlag) {
                _rightRelease = true;
            } else if (_rightRelease && gamepadRightReleaseFlag) {
                _rightRelease = false;
                gamepadRightReleaseFlag = false;
            }
        } else {
            if (!_rightRelease && !gamepadRightReleaseFlag) {
                gamepadRightReleaseFlag = true;
            }
        }

        _confirmRelease = Input.GetKeyUp(ControlsConfig.GamepadConfirm);
        _backRelease = Input.GetKeyUp(ControlsConfig.GamepadBack);
        _menuRelease = Input.GetKeyUp(ControlsConfig.GamepadMenu);
        _attackRelease = Input.GetKeyUp(ControlsConfig.GamepadAttack);
        _pauseRelease = Input.GetKeyUp(ControlsConfig.GamepadPause);
        _selectRelease = Input.GetKeyUp(ControlsConfig.GamepadSelect);

        //Set Horizontal and Vertical Axis Values
        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis = Input.GetAxis("Vertical");

        //Set Directional Press flags
        if (_horizontalAxis > joystickThreshold) {
            _right = true;
        } else if (_horizontalAxis < -joystickThreshold) {
            _left = true;
        } else {
            _left = false;
            _right = false;
        }

        if (_verticalAxis > joystickThreshold) {
            _up = true;
        } else if (_verticalAxis < -joystickThreshold) {
            _down = true;
        } else {
            _up = false;
            _down = false;
        }
    }

    private void Keyboard() {
        //Set key flags
        _up = Input.GetKey(ControlsConfig.KeyboardUp);
        _left = Input.GetKey(ControlsConfig.KeyboardLeft);
        _down = Input.GetKey(ControlsConfig.KeyboardDown);
        _right = Input.GetKey(ControlsConfig.KeyboardRight);
        _confirm = Input.GetKey(ControlsConfig.KeyboardConfirm);
        _back = Input.GetKey(ControlsConfig.KeyboardBack);
        _menu = Input.GetKey(ControlsConfig.KeyboardMenu);
        _attack = Input.GetKey(ControlsConfig.KeyboardAttack);
        _pause = Input.GetKey(ControlsConfig.KeyboardPause);
        _select = Input.GetKey(ControlsConfig.KeyboardSelect);

        //Set key press flags
        _upPress = Input.GetKeyDown(ControlsConfig.KeyboardUp);
        _leftPress = Input.GetKeyDown(ControlsConfig.KeyboardLeft);
        _downPress = Input.GetKeyDown(ControlsConfig.KeyboardDown);
        _rightPress = Input.GetKeyDown(ControlsConfig.KeyboardRight);
        _confirmPress = Input.GetKeyDown(ControlsConfig.KeyboardConfirm);
        _backPress = Input.GetKeyDown(ControlsConfig.KeyboardBack);
        _menuPress = Input.GetKeyDown(ControlsConfig.KeyboardMenu);
        _attackPress = Input.GetKeyDown(ControlsConfig.KeyboardAttack);
        _pausePress = Input.GetKeyDown(ControlsConfig.KeyboardPause);
        _selectPress = Input.GetKeyDown(ControlsConfig.KeyboardSelect);

        //Set key release flags
        _upRelease = Input.GetKeyUp(ControlsConfig.KeyboardUp);
        _leftRelease = Input.GetKeyUp(ControlsConfig.KeyboardLeft);
        _downRelease = Input.GetKeyUp(ControlsConfig.KeyboardDown);
        _rightRelease = Input.GetKeyUp(ControlsConfig.KeyboardRight);
        _confirmRelease = Input.GetKeyUp(ControlsConfig.KeyboardConfirm);
        _backRelease = Input.GetKeyUp(ControlsConfig.KeyboardBack);
        _menuRelease = Input.GetKeyUp(ControlsConfig.KeyboardMenu);
        _attackRelease = Input.GetKeyUp(ControlsConfig.KeyboardAttack);
        _pauseRelease = Input.GetKeyUp(ControlsConfig.KeyboardPause);
        _selectRelease = Input.GetKeyUp(ControlsConfig.KeyboardSelect);

        //Set Horizontal and Vertical Axis Values
        float tempHor;
        float tempVer;
        if (_left && _right) {
            tempHor = 0;
        } else if (_left) {
            tempHor = -1;
        } else if (_right) {
            tempHor = 1;
        } else {
            tempHor = 0;
        }

        if (_up && _down) {
            tempVer = 0;
        } else if (_down) {
            tempVer = -1;
        } else if (_up) {
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
                    inputListener += Gamepad;
                    inputListener -= Keyboard;
                }
            } else {
                if (_isGamepadConnected) {
                    _isGamepadConnected = false;
                    inputListener -= Gamepad;
                    inputListener += Keyboard;
                }
            }
        }
    }
}

