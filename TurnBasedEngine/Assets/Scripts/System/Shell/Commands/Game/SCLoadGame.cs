namespace BF2D.Game
{
    public class SCLoadGame
    {
        const string useage = "Useage: loadgame (optional ->) [saveID]";

        public static void Run(params string[] arguments)
        {
            if (arguments.Length > 2)
            {
                ShCtx.One.LogWarning(SCLoadGame.useage);
                return;
            }

            if (arguments.Length == 1)
            {
                if (GameCtx.One.ReloadGame())
                    ShCtx.One.Log($"Reloaded save file.");

                return;
            }

            string saveFileID = arguments[1];

            if (GameCtx.One.LoadGame(saveFileID))
                ShCtx.One.Log($"Loaded save at ID '{saveFileID}'.");
        }
    }
}