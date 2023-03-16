using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D;
using BF2D.UI;
using System.IO;
using UnityEngine.Networking;

namespace BF2D
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private DialogTextboxControl dialogTextboxController = null;
        [SerializeField] private float movementSpeed = 2;

        private float horizontalAxis;
        private float verticalAxis;

        private Action state;

        private void Start()
        {
            //this.state += Move;
            Terminal.IO.Log(Application.persistentDataPath);
            Terminal.IO.Log(Application.streamingAssetsPath);
        }

        private void Update()
        {
            this.state?.Invoke();
        }

        private void Move()
        {
            this.horizontalAxis = InputManager.Instance.HorizontalAxis;
            this.verticalAxis = InputManager.Instance.VerticalAxis;
            Vector3 moveFactor = new Vector3(this.horizontalAxis, this.verticalAxis, 0);
            Vector3 newPosition = transform.localPosition + (moveFactor * movementSpeed * Time.deltaTime);
            transform.localPosition = newPosition;
        }

        public void Print(string text)
        {
            Terminal.IO.Log(text);
        }

        public void CallTestDialogs()
        {
            this.dialogTextboxController.Textbox.Dialog("test", true, 2);
            this.dialogTextboxController.Textbox.Message("[N:Mr. Cool Guy]Hey hi I'm Mr. Cool Guy.", false);
            this.dialogTextboxController.Textbox.Dialog(new List<string> {
                "[N:Jim]Hi",
                "[N:-1]Hello",
                "[N:Giuseppe]Whaddup[E]"
            },
            false,
            0
            );
        }
    }
}

