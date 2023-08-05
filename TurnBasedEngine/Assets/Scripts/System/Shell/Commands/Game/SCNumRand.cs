using BF2D.Utilities;

namespace BF2D.Game
{
    public class SCNumRand
    {
        public static void Run(params string[] arguments)
        {
            const string warning = "Useage: numrand [numRandExpression]";

            if (arguments.Length > 2 || arguments.Length < 2)
            {
                ShCtx.One.LogWarning(warning);
                return;
            }

            NumRand numRand = new();
            ShCtx.One.Log(numRand.Calculate(arguments[1]));
        }
    }
}