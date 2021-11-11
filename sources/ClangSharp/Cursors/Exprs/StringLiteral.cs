// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StringLiteral : Expr
    {
        internal StringLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StringLiteral, CX_StmtClass.CX_StmtClass_StringLiteral)
        {
        }

        public CX_CharacterKind Kind => Handle.CharacterLiteralKind;

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
                    case CX_CharacterKind.CX_CLK_Ascii:
                    case CX_CharacterKind.CX_CLK_UTF8:
                    {
                        return new ReadOnlySpan<byte>(pCString, length).AsString();
                    }

                    case CX_CharacterKind.CX_CLK_Wide:
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            goto case CX_CharacterKind.CX_CLK_UTF16;
                        }

                        goto case CX_CharacterKind.CX_CLK_UTF32;
                    }

                    case CX_CharacterKind.CX_CLK_UTF16:
                    {
                        return new ReadOnlySpan<ushort>(pCString, length).AsString();
                    }

                    case CX_CharacterKind.CX_CLK_UTF32:
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
}
