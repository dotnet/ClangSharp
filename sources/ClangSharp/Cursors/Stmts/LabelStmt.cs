// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class LabelStmt : ValueStmt
{
    private readonly Lazy<LabelDecl> _decl;

    internal LabelStmt(CXCursor handle) : base(handle, CXCursor_LabelStmt, CX_StmtClass_LabelStmt)
    {
        Debug.Assert(NumChildren is 1);
        _decl = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl Decl => _decl.Value;

    public string Name => Handle.Name.CString;

    public Stmt SubStmt => Children[0];
}
