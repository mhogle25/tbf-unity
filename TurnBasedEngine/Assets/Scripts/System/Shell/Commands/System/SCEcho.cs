namespace BF2D
{
    public class SCEcho
    {
        public static void Run(params string[] arguments)
        {
            if (arguments.Length < 2)
                return;

            for (int i = 1; i < arguments.Length; i++)
            {
                ShCtx.One.Log(arguments[i]);
            }
        }
    }
}