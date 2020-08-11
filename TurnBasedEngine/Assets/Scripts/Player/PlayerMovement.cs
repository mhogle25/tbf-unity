using System;
//using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.2f;

    private Action actions;
    private void Start() {
        actions += Move;
    }
    private void Update() {
        actions();
    }

    private void Move() {
        Vector3 moveFactor = new Vector3(InputManager.HorizontalAxis, InputManager.VerticalAxis, 0);
        Vector3 newPosition = transform.localPosition + (moveFactor * speed);
        transform.localPosition = newPosition;
    }
}
