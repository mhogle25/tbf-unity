namespace BF2D.Game
{
    public class TcLoadGame
    {
        const string useage = "Useage: loadgame (optional ->) [saveID]";

        public static void Run(string[] arguments)
        {
            if (arguments.Length > 2)
            {
                Terminal.IO.LogWarning(TcLoadGame.useage);
                return;
            }

            if (arguments.Length == 1)
            {
                if (GameCtx.Instance.ReloadGame())
                    Terminal.IO.Log($"Reloaded save file.");

                return;
            }

            string saveFileID = arguments[1];

            if (GameCtx.Instance.LoadGame(saveFileID))
                Terminal.IO.Log($"Loaded save at ID '{saveFileID}'.");
        }
    }
}