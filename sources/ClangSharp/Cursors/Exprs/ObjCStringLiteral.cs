// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCStringLiteral : Expr
    {
        internal ObjCStringLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCStringLiteral, CX_StmtClass.CX_StmtClass_ObjCStringLiteral)
        {
            Debug.Assert(NumChildren is 1);
        }

        public StringLiteral String => (StringLiteral)Children[0];
    }
}
