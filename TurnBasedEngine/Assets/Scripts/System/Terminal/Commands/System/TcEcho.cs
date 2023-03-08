namespace BF2D
{
    public class TcEcho
    {
        public static void Run(string[] arguments)
        {
            if (arguments.Length < 2)
                return;

            for (int i = 1; i < arguments.Length; i++)
            {
                Terminal.IO.LogQuiet(arguments[i]);
            }
        }
    }
}