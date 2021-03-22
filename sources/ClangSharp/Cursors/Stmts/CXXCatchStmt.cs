// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXCatchStmt : Stmt
    {
        private readonly Lazy<Type> _caughtType;
        private readonly Lazy<VarDecl> _exceptionDecl;

        internal CXXCatchStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXCatchStmt, CX_StmtClass.CX_StmtClass_CXXCatchStmt)
        {
            Debug.Assert(NumChildren is 1);

            _caughtType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
            _exceptionDecl = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
        }

        public Type CaughtType => _caughtType.Value;

        public VarDecl ExceptionDecl => _exceptionDecl.Value;

        public Stmt HandlerBlock => Children[0];
    }
}
