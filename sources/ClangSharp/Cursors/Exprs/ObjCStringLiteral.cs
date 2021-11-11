// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
