// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class LabelDecl : NamedDecl
{
    private readonly Lazy<LabelStmt> _stmt;

    internal LabelDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Label)
    {
        _stmt = new Lazy<LabelStmt>(() => TranslationUnit.GetOrCreate<LabelStmt>(Handle.GetExpr(0)));
    }

    public LabelStmt Stmt => _stmt.Value;
}
