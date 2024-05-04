// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_StringKind;

namespace ClangSharp;

public sealed class StringLiteral : Expr
{
    internal StringLiteral(CXCursor handle) : base(handle, CXCursor_StringLiteral, CX_StmtClass_StringLiteral)
    {
    }

    public CX_StringKind Kind => Handle.StringLiteralKind;

    public unsafe string String
    {
        get
        {
            var pCString = clang.getCString(Handle.StringLiteralValue);

            if (pCString is null)
            {
                return string.Empty;
            }

            var constantArrayType = (ConstantArrayType)Type;
            var length = unchecked((int)constantArrayType.Size - 1);

            switch (Kind)
            {
                case CX_SLK_Ordinary:
                case CX_SLK_UTF8:
                {
                    return new ReadOnlySpan<byte>(pCString, length).AsString();
                }

                case CX_SLK_Wide:
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        goto case CX_SLK_UTF16;
                    }

                    goto case CX_SLK_UTF32;
                }

                case CX_SLK_UTF16:
                {
                    return new ReadOnlySpan<ushort>(pCString, length).AsString();
                }

                case CX_SLK_UTF32:
                {
                    return new ReadOnlySpan<uint>(pCString, length).AsString();
                }

                default:
                {
                    return string.Empty;
                }
            }
        }
    }
}
