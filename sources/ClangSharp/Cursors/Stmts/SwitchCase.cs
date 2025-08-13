// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class SwitchCase : Stmt
{
    private readonly ValueLazy<SwitchCase> _nextSwitchCase;

    private protected SwitchCase(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastSwitchCase or < CX_StmtClass_FirstSwitchCase)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _nextSwitchCase = new ValueLazy<SwitchCase>(() => TranslationUnit.GetOrCreate<SwitchCase>(Handle.NextSwitchCase));
    }

    public SwitchCase NextSwitchCase => _nextSwitchCase.Value;

    public Stmt? SubStmt
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
