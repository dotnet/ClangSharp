// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class CXXConstructExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _args;
    private readonly ValueLazy<CXXConstructorDecl> _constructor;

    internal CXXConstructExpr(CXCursor handle) : this(handle, CXCursor_CallExpr, CX_StmtClass_CXXConstructExpr)
    {
    }

    private protected CXXConstructExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastCXXConstructExpr or < CX_StmtClass_FirstCXXConstructExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
        Debug.Assert(NumChildren == NumArgs);

        _args = LazyList.Create<Expr, Stmt>(_children);
        _constructor = new ValueLazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.Referenced));
    }

    public IReadOnlyList<Expr> Args => _args;

    public CXXConstructorDecl Constructor => _constructor.Value;

    public CX_ConstructionKind ConstructionKind => Handle.ConstructionKind;

    public bool HadMultipleCandidates => Handle.HadMultipleCandidates;

    public bool IsElidable => Handle.IsElidable;

    public bool IsListInitialization => Handle.IsListInitialization;

    public bool IsStdInitListInitialization => Handle.IsStdInitListInitialization;

    public uint NumArgs => (uint)Handle.NumArguments;

    public bool RequiresZeroInitialization => Handle.RequiresZeroInitialization;
}
