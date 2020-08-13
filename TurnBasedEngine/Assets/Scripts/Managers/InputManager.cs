using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool IsGamepadConnected { get; private set; } = false;
    public static float HorizontalAxis { get; private set; } = 0;
    public static float VerticalAxis { get; private set; } = 0;
    public static bool Up { get; private set; } = false;
    public static bool Left { get; private set; } = false;
    public static bool Down { get; private set; } = false;
    public static bool Right { get; private set; } = false;
    public static bool Confirm { get; private set; } = false;
    public static bool Back { get; private set; } = false;
    public static bool Menu { get; private set; } = false;
    public static bool Attack { get; private set; } = false;
    public static bool Pause { get; private set; } = false;
    public static bool Select { get; private set; } = false;


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
        Confirm = Input.GetKey(ControlsConfig.GamepadConfirm);
        Back = Input.GetKey(ControlsConfig.GamepadBack);
        Menu = Input.GetKey(ControlsConfig.GamepadMenu);
        Attack = Input.GetKey(ControlsConfig.GamepadAttack);
        Pause = Input.GetKey(ControlsConfig.GamepadPause);
        Select = Input.GetKey(ControlsConfig.GamepadSelect);

        //Set Horizontal and Vertical Axis Values
        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");

        //Set Directional Press flags
        if (HorizontalAxis > joystickThreshold) {
            Right = true;
        } else if (HorizontalAxis < -joystickThreshold) {
            Left = true;
        } else {
            Left = false;
            Right = false;
        }

        if (VerticalAxis > joystickThreshold) {
            Up = true;
        } else if (VerticalAxis < -joystickThreshold) {
            Down = true;
        } else {
            Up = false;
            Down = false;
        }
    }

    private void Keyboard() {
        //Set key press flags
        Up = Input.GetKey(ControlsConfig.KeyboardUp);
        Left = Input.GetKey(ControlsConfig.KeyboardLeft);
        Down = Input.GetKey(ControlsConfig.KeyboardDown);
        Right = Input.GetKey(ControlsConfig.KeyboardRight);
        Confirm = Input.GetKey(ControlsConfig.KeyboardConfirm);
        Back = Input.GetKey(ControlsConfig.KeyboardBack);
        Menu = Input.GetKey(ControlsConfig.KeyboardMenu);
        Attack = Input.GetKey(ControlsConfig.KeyboardAttack);
        Pause = Input.GetKey(ControlsConfig.KeyboardPause);
        Select = Input.GetKey(ControlsConfig.KeyboardSelect);

        //Set Horizontal and Vertical Axis Values
        float tempHor;
        float tempVer;
        if (Left && Right) {
            tempHor = 0;
        } else if (Left) {
            tempHor = -1;
        } else if (Right) {
            tempHor = 1;
        } else {
            tempHor = 0;
        }

        if (Up && Down) {
            tempVer = 0;
        } else if (Down) {
            tempVer = -1;
        } else if (Up) {
            tempVer = 1;
        } else {
            tempVer = 0;
        }

        //Calculate distances for diagonals (unit circle shit)
        if (tempVer != 0 && tempHor != 0) {
            tempHor *= (Mathf.Sqrt(2) / 2);
            tempVer *= (Mathf.Sqrt(2) / 2);
        }

        HorizontalAxis = tempHor;
        VerticalAxis = tempVer;
    }

    private void GamepadConnected() {
        //Check if gamepad is connected
        string[] gamepadNames = Input.GetJoystickNames();
        if (gamepadNames.Length > 0) {
            if (gamepadNames[0] != string.Empty) {
                if (!IsGamepadConnected) {
                    IsGamepadConnected = true;
                    inputListener += Gamepad;
                    inputListener -= Keyboard;
                }
            } else {
                if (IsGamepadConnected) {
                    IsGamepadConnected = false;
                    inputListener -= Gamepad;
                    inputListener += Keyboard;
                }
            }
        }
    }
}
