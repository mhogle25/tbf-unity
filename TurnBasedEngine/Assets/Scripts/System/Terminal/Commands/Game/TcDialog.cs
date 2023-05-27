using System.Collections.Generic;

namespace BF2D.Game
{
    public class TcDialog
    {
        public static void Run(string[] arguments)
        {
            const string warningMessage = "Useage: dialog [startingIndex] [length] [line1] [line2]... (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 4)
            {
                Terminal.IO.LogWarning(warningMessage);
                return;
            }

            int startingIndex;
            try
            {
                startingIndex = int.Parse(arguments[1]);
            }
            catch
            {
                Terminal.IO.LogWarning(warningMessage);
                return;
            }

            int length;
            try
            {
                length = int.Parse(arguments[2]);
            }
            catch
            {
                Terminal.IO.LogWarning(warningMessage);
                return;
            }

            if (length > arguments.Length - 3)
            {
                Terminal.IO.LogWarning("Length was greater than the number of lines. " + warningMessage);
                return;
            }

            if (startingIndex >= length)
            {
                Terminal.IO.LogWarning("Starting index was outside the range of the dialog. " + warningMessage);
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

            GameInfo.Instance.SystemTextbox.Textbox.Dialog(dialog, false, startingIndex, null, inserts.ToArray());
            Terminal.IO.Log("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }
    }
}