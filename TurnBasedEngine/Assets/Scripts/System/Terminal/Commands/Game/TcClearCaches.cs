namespace BF2D.Game
{
    public class TcClearCaches
    {
        public static void Run(string[] arguments)
        {
            GameCtx.Instance.ClearCaches();
        }
    }
}