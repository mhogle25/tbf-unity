using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0.2f;

    private Action actions;
    private void Start() {
        actions += Move;
    }
    private void Update() {
        actions();
    }

    private void Move() {
        Vector3 moveFactor = new Vector3(InputManager.HorizontalAxis, InputManager.VerticalAxis, 0);
        Vector3 newPosition = transform.localPosition + (moveFactor * movementSpeed);
        transform.localPosition = newPosition;
    }
}
