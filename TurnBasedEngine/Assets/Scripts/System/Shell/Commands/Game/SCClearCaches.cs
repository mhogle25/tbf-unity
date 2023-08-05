namespace BF2D.Game
{
    public class SCClearCaches
    {
        public static void Run(params string[] arguments)
        {
            GameCtx.One.ClearCaches();
        }
    }
}