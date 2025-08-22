// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXCatchStmt : Stmt
{
    private readonly ValueLazy<Type> _caughtType;
    private readonly ValueLazy<VarDecl> _exceptionDecl;

    internal CXXCatchStmt(CXCursor handle) : base(handle, CXCursor_CXXCatchStmt, CX_StmtClass_CXXCatchStmt)
    {
        Debug.Assert(NumChildren is 1);

        _caughtType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _exceptionDecl = new ValueLazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
    }

    public Type CaughtType => _caughtType.Value;

    public VarDecl ExceptionDecl => _exceptionDecl.Value;

    public Stmt HandlerBlock => Children[0];
}
