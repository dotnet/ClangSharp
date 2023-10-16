// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using ClangSharp.Abstractions;

namespace ClangSharp;

internal static class StringExtensions
{
    public static string Unquote(this string str)
        => str.StartsWith('\"') && str.EndsWith('\"') && !str.EndsWith("\\\"", StringComparison.Ordinal)
            ? str[1..^1]
            : str;

    public static string NormalizePath(this string str)
        => str.Replace('\\', '/').Replace("//", "/", StringComparison.Ordinal);

    public static string NormalizeFullPath(this string str)
        => string.IsNullOrWhiteSpace(str) ? str : Path.GetFullPath(str).NormalizePath();

    public static string AsString(this AccessSpecifier value) => value switch {
        AccessSpecifier.Public => "public",
        AccessSpecifier.Protected => "protected",
        AccessSpecifier.ProtectedInternal => "protected internal",
        AccessSpecifier.Internal => "internal",
        AccessSpecifier.PrivateProtected => "private protected",
        AccessSpecifier.Private => "private",
        AccessSpecifier.None => "public",
        _ => "public"
    };

    public static string AsString(this CallConv value, bool isForFnPtr) => value switch
    {
        CallConv.Winapi => "Winapi",
        CallConv.Cdecl => "Cdecl",
        CallConv.StdCall => isForFnPtr ? "Stdcall" : "StdCall",
        CallConv.ThisCall => isForFnPtr ? "Thiscall" : "ThisCall",
        CallConv.FastCall => isForFnPtr ? "Fastcall" : "FastCall",
        CallConv.MemberFunction => "MemberFunction",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
