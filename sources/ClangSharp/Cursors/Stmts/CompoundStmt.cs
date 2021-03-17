// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
                foreach (var B in Body.Reverse())
                {
                    if (B is not NullStmt)
                    {
                        return B;
                    }
                }
                return BodyBack;
            }
        }
    }
}
