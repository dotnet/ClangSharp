// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class GotoStmt : Stmt
{
    private readonly Lazy<LabelDecl> _label;

    internal GotoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_GotoStmt, CX_StmtClass.CX_StmtClass_GotoStmt)
    {
        Debug.Assert(NumChildren is 0);
        _label = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl Label => _label.Value;
}
