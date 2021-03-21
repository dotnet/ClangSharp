// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
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
}
