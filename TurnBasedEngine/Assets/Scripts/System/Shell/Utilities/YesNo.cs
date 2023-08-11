namespace BF2D
{
    public static class YesNo
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
                ShCtx.One.LogError($"Error: Unknown argument passed, expected (Y|N)");
                return false;
            }

            return true;
        }

        public static bool Yes(string input)
        {
            if (System.Linq.Enumerable.Contains(YesNo.yesBank, input.ToLower().Trim()))
                return true;

            return false;
        }

        public static bool No(string input)
        {
            if (System.Linq.Enumerable.Contains(YesNo.noBank, input.ToLower().Trim()))
                return true;

            return false;
        }
    }
}