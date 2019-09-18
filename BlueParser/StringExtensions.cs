namespace BlueParser
{
    internal static class StringExtensions
    {
        public static bool SubstringEquals(this string original, int pos, string @string) => original.Substring(pos).StartsWith(@string);
    }
}
