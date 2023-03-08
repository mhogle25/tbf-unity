using System.Collections.Generic;

namespace BF2D.Game
{
    public class TcDialogKey
    {
        public static void Run(string[] arguments)
        {
            const string warningMessage = "Useage: dialogkey [startingIndex] [key] (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 3)
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            int startingIndex;
            try
            {
                startingIndex = int.Parse(arguments[1]);
            }
            catch
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            List<string> inserts = new();
            for (int i = 3; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            GameInfo.Instance.SystemTextbox.Textbox.Dialog(arguments[2], startingIndex, null, inserts);
            Terminal.IO.LogQuiet("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }
    }
}