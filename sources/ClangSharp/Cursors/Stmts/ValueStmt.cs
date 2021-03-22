// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueStmt : Stmt
    {
        private protected ValueStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastValueStmt < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstValueStmt))
            {
                throw new ArgumentException(nameof(handle));
            }
        }

        public Expr ExprStmt
        {
            get
            {
                Stmt S = this;
                do
                {
                    if (S is Expr E)
                    {
                        return E;
                    }

                    if (S is LabelStmt LS)
                    {
                        S = LS.SubStmt;
                    }
                    else if (S is AttributedStmt AS)
                    {
                        S = AS.SubStmt;
                    }
                    else
                    {
                        Debug.Fail("unknown kind of ValueStmt");
                    }
                } while (S is ValueStmt);

                return null;
            }
        }
    }
}
