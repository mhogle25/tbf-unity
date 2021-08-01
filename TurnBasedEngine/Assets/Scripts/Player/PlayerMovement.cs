using System;
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
        DialogTextbox.Instance.Dialog("test", 2);

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
