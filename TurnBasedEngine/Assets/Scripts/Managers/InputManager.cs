using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool IsGamepadConnected { get { return _isGamepadConnected; } }
    private static bool _isGamepadConnected = false;
    public static float HorizontalAxis { get { return _horizontalAxis; } }
    private static float _horizontalAxis = 0;
    public static float VerticalAxis { get { return _verticalAxis; } }
    private static float _verticalAxis = 0;
    public static bool Up { get { return _up; } }
    private static bool _up = false;
    public static bool Left { get { return _down; } }
    private static bool _down = false;
    public static bool Down { get { return _left; } }
    private static bool _left = false;
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

    private Action inputListener;
    private float joystickThreshold = 0.5f;

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
        //Set Key Press Flags
        _confirm = Input.GetKey(ControlsConfig.GamepadConfirm);
        _back = Input.GetKey(ControlsConfig.GamepadBack);
        _menu = Input.GetKey(ControlsConfig.GamepadMenu);
        _attack = Input.GetKey(ControlsConfig.GamepadAttack);
        _pause = Input.GetKey(ControlsConfig.GamepadPause);
        _select = Input.GetKey(ControlsConfig.GamepadSelect);

        //Set Horizontal and Vertical Axis Values
        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis = Input.GetAxis("Vertical");

        //Set Directional Press flags
        if (_horizontalAxis > joystickThreshold) {
            _right = true;
        } else if (_horizontalAxis < -joystickThreshold) {
            _down = true;
        } else {
            _down = false;
            _right = false;
        }

        if (_verticalAxis > joystickThreshold) {
            _up = true;
        } else if (_verticalAxis < -joystickThreshold) {
            _left = true;
        } else {
            _up = false;
            _left = false;
        }
    }

    private void Keyboard() {
        //Set key press flags
        _up = Input.GetKey(ControlsConfig.KeyboardUp);
        _down = Input.GetKey(ControlsConfig.KeyboardLeft);
        _left = Input.GetKey(ControlsConfig.KeyboardDown);
        _right = Input.GetKey(ControlsConfig.KeyboardRight);
        _confirm = Input.GetKey(ControlsConfig.KeyboardConfirm);
        _back = Input.GetKey(ControlsConfig.KeyboardBack);
        _menu = Input.GetKey(ControlsConfig.KeyboardMenu);
        _attack = Input.GetKey(ControlsConfig.KeyboardAttack);
        _pause = Input.GetKey(ControlsConfig.KeyboardPause);
        _select = Input.GetKey(ControlsConfig.KeyboardSelect);

        //Set Horizontal and Vertical Axis Values
        float tempHor;
        float tempVer;
        if (_down && _right) {
            tempHor = 0;
        } else if (_down) {
            tempHor = -1;
        } else if (_right) {
            tempHor = 1;
        } else {
            tempHor = 0;
        }

        if (_up && _left) {
            tempVer = 0;
        } else if (_left) {
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
