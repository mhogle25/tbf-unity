using System.Collections.Generic;

namespace BF2D.Game
{
    public class SCDialogKey
    {
        public static void Run(params string[] arguments)
        {
            const string warningMessage = "Useage: dialogkey [startingIndex] [key] (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 3)
            {
                ShCtx.One.LogWarning(warningMessage);
                return;
            }

            int startingIndex;
            try
            {
                startingIndex = int.Parse(arguments[1]);
            }
            catch
            {
                ShCtx.One.LogWarning(warningMessage);
                return;
            }

            List<string> inserts = new();
            for (int i = 3; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            UI.UICtx.One.SystemTextbox.Dialog(arguments[2], startingIndex, null, inserts.ToArray());
            ShCtx.One.Log("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }
    }
}