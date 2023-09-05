using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public static class YesNo
    {
        private static readonly HashSet<string> yesBank = new()
        {
            "yes",
            "y",
        };

        private static readonly HashSet<string> noBank = new()
        {
            "no",
            "n",
        };

        public static bool Either(string input)
        {
            if (!Yes(input) && !No(input))
            {
                Debug.LogError($"[YesNo:Either] Unknown argument passed, expected (Y|N)");
                return false;
            }

            return true;
        }

        public static bool Yes(string input) => YesNo.yesBank.Contains(input.Wash());

        public static bool No(string input) => YesNo.noBank.Contains(input.Wash());

        private static string Wash(this string value) => value.ToLower().Trim();
    }
}