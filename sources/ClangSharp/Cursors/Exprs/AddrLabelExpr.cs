// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class AddrLabelExpr : Expr
{
    private ValueLazy<AddrLabelExpr, LabelDecl> _label;

    internal unsafe AddrLabelExpr(CXCursor handle) : base(handle, CXCursor_AddrLabelExpr, CX_StmtClass_AddrLabelExpr)
    {
        Debug.Assert(NumChildren is 0);
        _label = new ValueLazy<AddrLabelExpr, LabelDecl>(&LabelFactory);
    }

    public LabelDecl Label => _label.GetValue(this);

    private static unsafe LabelDecl LabelFactory(AddrLabelExpr self) => self.TranslationUnit.GetOrCreate<LabelDecl>(self.Handle.Referenced);
}
