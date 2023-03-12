using System.Text.RegularExpressions;

namespace BF2D.Utilities
{
    public static class StringExtensionMethods
    {
        private static readonly Regex replaceBrackets = new(@"\[[^)]*\]");
        private static readonly Regex replaceExtraSpace = new(@"\s{2,}");

        public static string Wash(this string value)
        {
            string text = StringExtensionMethods.replaceBrackets.Replace(value, "");
            return StringExtensionMethods.replaceExtraSpace.Replace(text, " ");
        }
    }
}