//using System;
using System;
using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static float HorizontalAxis { get; private set; } = 0;
    public static float VerticalAxis { get; private set; } = 0;
    public static bool UpPressed { get; private set; } = false;
    public static bool LeftPressed { get; private set; } = false;
    public static bool DownPressed { get; private set; } = false;
    public static bool RightPressed { get; private set; } = false;
    public static bool SelectPressed { get; private set; } = false;
    public static bool BackPressed { get; private set; } = false;

    private Action inputListener;

    private void Start() {
        inputListener += Keyboard;
    }

    private void Update() {
        inputListener();
    }

    private void Keyboard() {
        //Set key press flags
        UpPressed = Input.GetKey(ControlsConfig.ButtonUp);
        LeftPressed = Input.GetKey(ControlsConfig.ButtonLeft);
        DownPressed = Input.GetKey(ControlsConfig.ButtonDown);
        RightPressed = Input.GetKey(ControlsConfig.ButtonRight);
        SelectPressed = Input.GetKey(ControlsConfig.ButtonSelect);
        BackPressed = Input.GetKey(ControlsConfig.ButtonBack);

        //Set Horizontal and Vertical Axis Values
        float tempHor;
        float tempVer;
        if (LeftPressed && RightPressed) {
            tempHor = 0;
        } else if (LeftPressed) {
            tempHor = -1;
        } else if (RightPressed) {
            tempHor = 1;
        } else {
            tempHor = 0;
        }

        if (UpPressed && DownPressed) {
            tempVer = 0;
        } else if (DownPressed) {
            tempVer = -1;
        } else if (UpPressed) {
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
}
