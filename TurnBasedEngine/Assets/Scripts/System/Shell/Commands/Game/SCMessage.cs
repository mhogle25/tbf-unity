using System.Collections.Generic;

namespace BF2D.Game
{
    public class SCMessage
    {
        public static void Run(params string[] arguments)
        {
            if (arguments.Length < 2)
            {
                ShCtx.One.LogWarning("Useage: message [text] (optional ->) [insert1] [insert2]...");
                return;
            }

            List<string> inserts = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            UI.UICtx.One.SystemTextbox.Message(arguments[1], null, inserts.ToArray());
            ShCtx.One.Log("Pushed a message to the system textbox's queue. Run with 'textbox'.");
        }
    }
}