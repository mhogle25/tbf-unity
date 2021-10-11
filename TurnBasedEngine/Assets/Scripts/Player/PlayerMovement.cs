using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D;
using BF2D.Dialog;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2;

    private float _horizontalAxis;
    private float _verticalAxis;

    private Action _state;

    private void Start() {
        DialogTextbox.Instance.Dialog("test", 3);
        DialogTextbox.Instance.Message("[N:Mr. Cool Guy]Hey hi I'm Mr. Cool Guy.");
        DialogTextbox.Instance.Dialog(new List<string> {
                "[N:Jim]Hi",
                "[N:-1]Hello",
                "[N:Giuseppe]Whaddup[E]"
            },
            1
        );

        _state += Move;
    }

    private void Update() {
        if (_state != null)
            _state();
    }

    private void Move() {
        _horizontalAxis = InputManager.HorizontalAxis;
        _verticalAxis = InputManager.VerticalAxis;
        Vector3 moveFactor = new Vector3(_horizontalAxis, _verticalAxis, 0);
        Vector3 newPosition = transform.localPosition + (moveFactor * movementSpeed * Time.deltaTime);
        transform.localPosition = newPosition;
    }
}
