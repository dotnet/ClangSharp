namespace ClangSharp
{
    internal static class StringExtensions
    {
        public static string Unquote(this string str)
            => str.StartsWith("\"") && str.EndsWith("\"") && !str.EndsWith("\\\"")
                ? str.Substring(1, str.Length - 2)
                : str;
    }
}
