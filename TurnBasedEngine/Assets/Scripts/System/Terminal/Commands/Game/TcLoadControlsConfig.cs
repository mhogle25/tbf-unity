using BF2D.Enums;
using UnityEngine;

namespace BF2D.Game
{
    public class TcLoadControlsConfig
    {
        public static void Run(string[] arguments)
        {
            const string warningMessage = "Useage: loadconfig [keyboard OR gamepad] [fileID]";

            if (arguments.Length == 1)
            {
                GameCtx.Instance.LoadControlsConfig(InputController.Keyboard, Strings.System.Default);
                GameCtx.Instance.LoadControlsConfig(InputController.Gamepad, Strings.System.Default);
                return;
            }

            if (arguments.Length < 3 || arguments.Length > 3)
            {
                Debug.LogWarning(warningMessage);
                return;
            }

            string type = arguments[1];
            InputController controllerType;
            if (type == "keyboard")
                controllerType = InputController.Keyboard;
            else if (type == "gamepad")
                controllerType = InputController.Gamepad;
            else
            {
                Debug.LogWarning($"Unknown controller type '{arguments[1]}'. " + warningMessage);
                return;
            }

            GameCtx.Instance.LoadControlsConfig(controllerType, arguments[2]);
        }
    }
}