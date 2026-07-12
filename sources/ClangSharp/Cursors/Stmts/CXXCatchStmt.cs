// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXCatchStmt : Stmt
{
    private ValueLazy<CXXCatchStmt, Type> _caughtType;
    private ValueLazy<CXXCatchStmt, VarDecl> _exceptionDecl;

    internal unsafe CXXCatchStmt(CXCursor handle) : base(handle, CXCursor_CXXCatchStmt, CX_StmtClass_CXXCatchStmt)
    {
        Debug.Assert(NumChildren is 1);

        _caughtType = new ValueLazy<CXXCatchStmt, Type>(&CaughtTypeFactory);
        _exceptionDecl = new ValueLazy<CXXCatchStmt, VarDecl>(&ExceptionDeclFactory);
    }

    public Type CaughtType => _caughtType.GetValue(this);

    public VarDecl ExceptionDecl => _exceptionDecl.GetValue(this);

    public Stmt HandlerBlock => Children[0];

    private static unsafe VarDecl ExceptionDeclFactory(CXXCatchStmt self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.Referenced);

    private static unsafe Type CaughtTypeFactory(CXXCatchStmt self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
