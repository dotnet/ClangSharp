// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class AddrLabelExpr : Expr
{
    private readonly ValueLazy<LabelDecl> _label;

    internal AddrLabelExpr(CXCursor handle) : base(handle, CXCursor_AddrLabelExpr, CX_StmtClass_AddrLabelExpr)
    {
        Debug.Assert(NumChildren is 0);
        _label = new ValueLazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl Label => _label.Value;
}
