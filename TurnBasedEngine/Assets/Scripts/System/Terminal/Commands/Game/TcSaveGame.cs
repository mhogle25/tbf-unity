namespace BF2D.Game
{
    public class TcSaveGame
    {
        public static void Run(string[] arguments)
        {
            if (arguments.Length == 1)
            {
                GameCtx.Instance.SaveGame();
                return;
            }

            for (int i = 1; i < arguments.Length; i++)
            {
                string newSaveFileID = arguments[i];
                GameCtx.Instance.SaveGameAs(newSaveFileID);
            }
        }
    }
}