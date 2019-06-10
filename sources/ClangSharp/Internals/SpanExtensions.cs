using System;
using System.Text;

namespace ClangSharp
{
    internal static unsafe class SpanExtensions
    {
        public static string AsString(this Span<byte> self) => AsString((ReadOnlySpan<byte>)self);

        public static string AsString(this ReadOnlySpan<byte> self)
        {
            if (self.IsEmpty)
            {
                return string.Empty;
            }

            fixed (byte* pSelf = self)
            {
                return Encoding.UTF8.GetString(pSelf, self.Length);
            }
        }

        public static void ClangFree(this Span<byte> self) => ClangFree((ReadOnlySpan<byte>)self);

        public static void ClangFree(this ReadOnlySpan<byte> self)
        {
            fixed (byte* pSelf = self)
            {
                clang.free(pSelf);
            }
        }
    }
}
