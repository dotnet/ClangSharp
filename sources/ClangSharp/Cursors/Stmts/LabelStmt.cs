// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class LabelStmt : ValueStmt
{
    private readonly Lazy<LabelDecl> _decl;

    internal LabelStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_LabelStmt, CX_StmtClass.CX_StmtClass_LabelStmt)
    {
        Debug.Assert(NumChildren is 1);
        _decl = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl Decl => _decl.Value;

    public string Name => Handle.Name.CString;

    public Stmt SubStmt => Children[0];
}
