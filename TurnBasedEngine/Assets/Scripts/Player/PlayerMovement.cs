using System;
using UnityEngine;
using BF2D;
using BF2D.Dialog;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2;

    private float horizontalAxis;
    private float verticalAxis;

    private Action state;
    private void Start() {
        DialogTextbox.Instance.Dialog("test", 2);

        state += Move;
    }
    private void LateUpdate() {
        if (state != null)
            state();
    }

    private void Move() {
        horizontalAxis = InputManager.HorizontalAxis;
        verticalAxis = InputManager.VerticalAxis;
        Vector3 moveFactor = new Vector3(horizontalAxis, verticalAxis, 0);
        Vector3 newPosition = transform.localPosition + (moveFactor * movementSpeed * Time.deltaTime);
        transform.localPosition = newPosition;
    }
}
