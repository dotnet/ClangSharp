// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class BinaryConditionalOperator : AbstractConditionalOperator
{
    private readonly Lazy<OpaqueValueExpr> _opaqueValue;

    internal BinaryConditionalOperator(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_BinaryConditionalOperator)
    {
        Debug.Assert(NumChildren is 4);
        _opaqueValue = new Lazy<OpaqueValueExpr>(() => TranslationUnit.GetOrCreate<OpaqueValueExpr>(Handle.OpaqueValue));
    }

    public Expr Common => (Expr)Children[0];

    public new Expr Cond => (Expr)Children[1];

    public new Expr FalseExpr => (Expr)Children[3];

    public OpaqueValueExpr OpaqueValue => _opaqueValue.Value;

    public new Expr TrueExpr => (Expr)Children[2];
}
