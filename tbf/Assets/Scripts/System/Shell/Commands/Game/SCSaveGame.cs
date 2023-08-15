namespace BF2D.Game
{
    public class SCSaveGame
    {
        public static void Run(params string[] arguments)
        {
            if (arguments.Length == 1)
            {
                GameCtx.One.SaveGame();
                return;
            }

            for (int i = 1; i < arguments.Length; i++)
            {
                string newSaveFileID = arguments[i];
                GameCtx.One.SaveGameAs(newSaveFileID);
            }
        }
    }
}