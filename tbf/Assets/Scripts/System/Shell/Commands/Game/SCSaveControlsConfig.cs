using BF2D.Enums;
using UnityEngine;

namespace BF2D.Game
{
    public class SCSaveControlsConfig
    {
        public static void Run(params string[] arguments)
        {
            const string warningMessage = "Useage: saveconfig [keyboard OR gamepad] (optional ->) [fileID1] [fileID2]...";

            if (arguments.Length < 2)
            {
                Debug.LogWarning(warningMessage);
                return;
            }

            string type = arguments[1];
            type = type.ToLower();
            type = type.Trim();
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

            if (arguments.Length == 2)
            {
                GameCtx.One.SaveControlsConfig(controllerType);
                return;
            }

            for (int i = 2; i < arguments.Length; i++)
            {
                string newControlsConfigID = arguments[i];
                GameCtx.One.SaveControlsConfigAs(controllerType, newControlsConfigID);
            }
        }
    }
}