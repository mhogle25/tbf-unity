using BF2D.Utilities;

namespace BF2D.Game
{
    public class TcNumRand
    {
        public static void Run(string[] arguments)
        {
            const string warning = "Useage: numrand [numRandExpression]";

            if (arguments.Length > 2 || arguments.Length < 2)
            {
                Terminal.IO.LogWarning(warning);
                return;
            }

            NumRand numRand = new();
            Terminal.IO.Log(numRand.Calculate(arguments[1]));
        }
    }
}