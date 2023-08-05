using BF2D.Enums;
using UnityEngine;

namespace BF2D.Game
{
    public class SCLoadControlsConfig
    {
        public static void Run(params string[] arguments)
        {
            const string warningMessage = "Useage: loadconfig [keyboard OR gamepad] [fileID]";

            if (arguments.Length == 1)
            {
                GameCtx.One.LoadControlsConfig(InputController.Keyboard, Strings.System.Default);
                GameCtx.One.LoadControlsConfig(InputController.Gamepad, Strings.System.Default);
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

            GameCtx.One.LoadControlsConfig(controllerType, arguments[2]);
        }
    }
}