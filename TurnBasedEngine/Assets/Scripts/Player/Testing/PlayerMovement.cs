using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D;
using BF2D.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2;

    private float horizontalAxis;
    private float verticalAxis;

    private Action state;

    private void Start() {
        DialogTextbox.Instance.Dialog("test", 2);

        DialogTextbox.Instance.Message("[N:Mr. Cool Guy]Hey hi I'm Mr. Cool Guy.");
        
        DialogTextbox.Instance.Dialog(new List<string> {
                "[N:Jim]Hi",
                "[N:-1]Hello",
                "[N:Giuseppe]Whaddup[E]"
            },
            0
        );

        //this.state += Move;
        this.state += Listen;

    }   

    private void Update() {
        if (this.state != null)
            this.state();
    }

    private void Move() {
        this.horizontalAxis = InputManager.HorizontalAxis;
        this.verticalAxis = InputManager.VerticalAxis;
        Vector3 moveFactor = new Vector3(this.horizontalAxis, this.verticalAxis, 0);
        Vector3 newPosition = transform.localPosition + (moveFactor * movementSpeed * Time.deltaTime);
        transform.localPosition = newPosition;
    }

    private void Listen()
    {
        if (InputManager.ConfirmPress)
        {
            DialogTextbox.Instance.Continue();
        }
    }

    public void Print(string text)
    {
        Debug.Log(text);
    }
}
