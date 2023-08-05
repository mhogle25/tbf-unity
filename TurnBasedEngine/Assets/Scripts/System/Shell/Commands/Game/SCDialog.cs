using System.Collections.Generic;

namespace BF2D.Game
{
    public class SCDialog
    {
        public static void Run(params string[] arguments)
        {
            const string warningMessage = "Useage: dialog [startingIndex] [length] [line1] [line2]... (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 4)
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

            int length;
            try
            {
                length = int.Parse(arguments[2]);
            }
            catch
            {
                ShCtx.One.LogWarning(warningMessage);
                return;
            }

            if (length > arguments.Length - 3)
            {
                ShCtx.One.LogWarning("Length was greater than the number of lines. " + warningMessage);
                return;
            }

            if (startingIndex >= length)
            {
                ShCtx.One.LogWarning("Starting index was outside the range of the dialog. " + warningMessage);
                return;
            }

            List<string> dialog = new();
            for (int i = 3; i < length + 3; i++)
            {
                dialog.Add(arguments[i]);
            }

            List<string> inserts = new();
            for (int i = length + 3; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            UI.UICtx.One.SystemTextbox.Dialog(dialog, startingIndex, null, inserts.ToArray());
            ShCtx.One.Log("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }
    }
}