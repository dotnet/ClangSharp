// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Text;

namespace ClangSharp.Interop
{
    public static unsafe class SpanExtensions
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

        public static string AsString(this ReadOnlySpan<ushort> self)
        {
            if (self.IsEmpty)
            {
                return string.Empty;
            }

            fixed (ushort* pSelf = self)
            {
                return Encoding.Unicode.GetString((byte*)pSelf, self.Length * 2);
            }
        }

        public static string AsString(this ReadOnlySpan<uint> self)
        {
            if (self.IsEmpty)
            {
                return string.Empty;
            }

            fixed (uint* pSelf = self)
            {
                return Encoding.UTF32.GetString((byte*)pSelf, self.Length * 4);
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
