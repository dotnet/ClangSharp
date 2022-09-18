// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Abstractions;

namespace ClangSharp;

internal static class StringExtensions
{
    public static string Unquote(this string str)
        => str.StartsWith("\"") && str.EndsWith("\"") && !str.EndsWith("\\\"")
            ? str[1..^1]
            : str;

    public static string NormalizePath(this string str)
        => str.Replace('\\', '/').Replace("//", "/");

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

    public static string AsString(this CallingConvention value, bool isForFnPtr) => value switch
    {
        CallingConvention.Winapi => "Winapi",
        CallingConvention.Cdecl => "Cdecl",
        CallingConvention.StdCall => isForFnPtr ? "Stdcall" : "StdCall",
        CallingConvention.ThisCall => isForFnPtr ? "Thiscall" : "ThisCall",
        CallingConvention.FastCall => isForFnPtr ? "Fastcall" : "FastCall",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
