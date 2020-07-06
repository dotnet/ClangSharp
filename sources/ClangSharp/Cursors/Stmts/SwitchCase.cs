// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class SwitchCase : Stmt
    {
        private readonly Lazy<SwitchCase> _nextSwitchCase;
        private readonly Lazy<Stmt> _subStmt;

        private protected SwitchCase(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastSwitchCase < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstSwitchCase))
            {
                throw new ArgumentException(nameof(handle));
            }

            _nextSwitchCase = new Lazy<SwitchCase>(() => TranslationUnit.GetOrCreate<SwitchCase>(Handle.NextSwitchCase));
            _subStmt = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.SubStmt));
        }

        public SwitchCase NextSwitchCase => _nextSwitchCase.Value;

        public Stmt SubStmt => _subStmt.Value;
    }
}
