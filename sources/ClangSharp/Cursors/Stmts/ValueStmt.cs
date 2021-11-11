// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueStmt : Stmt
    {
        private protected ValueStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastValueStmt or < CX_StmtClass.CX_StmtClass_FirstValueStmt)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }

        public Expr ExprStmt
        {
            get
            {
                Stmt s = this;

                do
                {
                    if (s is Expr e)
                    {
                        return e;
                    }

                    if (s is LabelStmt ls)
                    {
                        s = ls.SubStmt;
                    }
                    else if (s is AttributedStmt @as)
                    {
                        s = @as.SubStmt;
                    }
                    else
                    {
                        Debug.Fail("unknown kind of ValueStmt");
                    }
                } while (s is ValueStmt);

                return null;
            }
        }
    }
}
