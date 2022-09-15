// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class AddrLabelExpr : Expr
{
    private readonly Lazy<LabelDecl> _label;

    internal AddrLabelExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_AddrLabelExpr, CX_StmtClass.CX_StmtClass_AddrLabelExpr)
    {
        Debug.Assert(NumChildren is 0);
        _label = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl Label => _label.Value;
}
