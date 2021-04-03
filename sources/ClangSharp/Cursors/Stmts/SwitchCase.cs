// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class SwitchCase : Stmt
    {
        private readonly Lazy<SwitchCase> _nextSwitchCase;

        private protected SwitchCase(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastSwitchCase or < CX_StmtClass.CX_StmtClass_FirstSwitchCase)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _nextSwitchCase = new Lazy<SwitchCase>(() => TranslationUnit.GetOrCreate<SwitchCase>(Handle.NextSwitchCase));
        }

        public SwitchCase NextSwitchCase => _nextSwitchCase.Value;

        public Stmt SubStmt
        {
            get
            {
                if (this is CaseStmt cs)
                {
                    return cs.SubStmt;
                }
                else if (this is DefaultStmt ds)
                {
                    return ds.SubStmt;
                }

                Debug.Fail("SwitchCase is neither a CaseStmt nor a DefaultStmt!");
                return null;
            }
        }
    }
}
