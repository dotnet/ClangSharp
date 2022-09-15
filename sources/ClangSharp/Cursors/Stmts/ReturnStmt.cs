// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ReturnStmt : Stmt
{
    private readonly Lazy<VarDecl> _nrvoCandidate;

    internal ReturnStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ReturnStmt, CX_StmtClass.CX_StmtClass_ReturnStmt)
    {
        Debug.Assert(NumChildren is 0 or 1);

        _nrvoCandidate = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
    }

    public VarDecl NRVOCandidate => _nrvoCandidate.Value;

    public Expr RetValue => NumChildren != 0 ? (Expr)Children[0] : null;
}
