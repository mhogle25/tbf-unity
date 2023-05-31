using System.Collections.Generic;

namespace BF2D
{
    public class YesNo
    {
        private static readonly string[] yesBank =
        {
            "yes",
            "y",
        };

        private static readonly string[] noBank =
        {
            "no",
            "n",
        };

        public static bool Either(string input)
        {
            if (!Yes(input) && !No(input))
            {
                Terminal.IO.LogError($"Error: Unknown argument passed, expected (Y|N)");
                return false;
            }

            return true;
        }

        public static bool Yes(string input)
        {
            if (System.Linq.Enumerable.Contains(YesNo.yesBank, input.ToLower()))
                return true;

            return false;
        }

        public static bool No(string input)
        {
            if (System.Linq.Enumerable.Contains(YesNo.noBank, input.ToLower()))
                return true;

            return false;
        }
    }
}