// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundStmt : Stmt
    {
        public CompoundStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundStmt, CX_StmtClass.CX_StmtClass_CompoundStmt)
        {
        }

        public IReadOnlyList<Stmt> Body => Children;

        public Stmt BodyBack => Children.LastOrDefault();

        public Stmt BodyFront => Children.FirstOrDefault();

        public uint Size => unchecked((uint)NumChildren);

        public Stmt StmtExprResult
        {
            get
            {
                foreach (var b in Body.Reverse())
                {
                    if (b is not NullStmt)
                    {
                        return b;
                    }
                }
                return BodyBack;
            }
        }
    }
}
