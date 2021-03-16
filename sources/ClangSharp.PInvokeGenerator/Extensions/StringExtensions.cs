using ClangSharp.Abstractions;

namespace ClangSharp
{
    internal static class StringExtensions
    {
        public static string Unquote(this string str)
            => str.StartsWith("\"") && str.EndsWith("\"") && !str.EndsWith("\\\"")
                ? str.Substring(1, str.Length - 2)
                : str;

        public static string AsString(this AccessSpecifier value) => value switch
        {
            AccessSpecifier.Public => "public",
            AccessSpecifier.Protected => "protected",
            AccessSpecifier.ProtectedInternal => "protected internal",
            AccessSpecifier.Internal => "internal",
            AccessSpecifier.PrivateProtected => "private protected",
            AccessSpecifier.Private => "private",
            _ => "public"
        };
    }
}
