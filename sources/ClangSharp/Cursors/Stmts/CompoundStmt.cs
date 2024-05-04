// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CompoundStmt(CXCursor handle) : Stmt(handle, CXCursor_CompoundStmt, CX_StmtClass_CompoundStmt)
{
    public IReadOnlyList<Stmt> Body => Children;

    public Stmt? BodyBack
    {
        get
        {
            var children = Children;
            return (children.Count != 0) ? children[^1] : null;
        }
    }

    public Stmt? BodyFront
    {
        get
        {
            var children = Children;
            return (children.Count != 0) ? children[0] : null;
        }
    }

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

            var bodyBack = BodyBack;
            Debug.Assert(bodyBack is not null);
            return bodyBack!;
        }
    }
}
