// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ReturnStmt : Stmt
{
    private ValueLazy<ReturnStmt, VarDecl> _nrvoCandidate;

    internal unsafe ReturnStmt(CXCursor handle) : base(handle, CXCursor_ReturnStmt, CX_StmtClass_ReturnStmt)
    {
        Debug.Assert(NumChildren is 0 or 1);

        _nrvoCandidate = new ValueLazy<ReturnStmt, VarDecl>(&NrvoCandidateFactory);
    }

    public VarDecl NRVOCandidate => _nrvoCandidate.GetValue(this);

    public Expr? RetValue => NumChildren != 0 ? (Expr)Children[0] : null;

    private static unsafe VarDecl NrvoCandidateFactory(ReturnStmt self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.Referenced);
}
