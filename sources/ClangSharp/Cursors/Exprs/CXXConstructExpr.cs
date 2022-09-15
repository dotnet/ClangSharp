// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp;

public class CXXConstructExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _args;
    private readonly Lazy<CXXConstructorDecl> _constructor;

    internal CXXConstructExpr(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXConstructExpr)
    {
    }

    private protected CXXConstructExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastCXXConstructExpr or < CX_StmtClass.CX_StmtClass_FirstCXXConstructExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
        Debug.Assert(NumChildren == NumArgs);

        _args = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        _constructor = new Lazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.Referenced));
    }

    public IReadOnlyList<Expr> Args => _args.Value;

    public CXXConstructorDecl Constructor => _constructor.Value;

    public CX_ConstructionKind ConstructionKind => Handle.ConstructionKind;

    public bool HadMultipleCandidates => Handle.HadMultipleCandidates;

    public bool IsElidable => Handle.IsElidable;

    public bool IsListInitialization => Handle.IsListInitialization;

    public bool IsStdInitListInitialization => Handle.IsStdInitListInitialization;

    public uint NumArgs => (uint)Handle.NumArguments;

    public bool RequiresZeroInitialization => Handle.RequiresZeroInitialization;
}
