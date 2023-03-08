using System.Collections.Generic;

namespace BF2D.Game
{
    public class TcMessage
    {
        public static void Run(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Terminal.IO.LogWarningQuiet("Useage: message [text] (optional ->) [insert1] [insert2]...");
                return;
            }

            List<string> inserts = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            GameInfo.Instance.SystemTextbox.Textbox.Message(arguments[1], null, inserts);
            Terminal.IO.LogQuiet("Pushed a message to the system textbox's queue. Run with 'textbox'.");
        }
    }
}