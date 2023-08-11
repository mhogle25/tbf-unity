using System.Text.RegularExpressions;

namespace BF2D.Utilities
{
    public static class StringExtensions
    {
        private static readonly Regex replaceBrackets = new(@"\[[^)]*\]");
        private static readonly Regex replaceExtraSpace = new(@"\s{2,}");

        public static string Wash(this string value) => StringExtensions.replaceBrackets.Replace(StringExtensions.replaceExtraSpace.Replace(value, " "), "");
    }
}