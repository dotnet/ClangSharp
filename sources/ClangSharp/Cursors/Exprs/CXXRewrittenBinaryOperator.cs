// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXRewrittenBinaryOperator : Expr
{
    private ValueLazy<CXXRewrittenBinaryOperator, Expr> _lhs;
    private ValueLazy<CXXRewrittenBinaryOperator, Expr> _rhs;

    internal unsafe CXXRewrittenBinaryOperator(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXRewrittenBinaryOperator)
    {
        Debug.Assert(NumChildren is 1);

        _lhs = new ValueLazy<CXXRewrittenBinaryOperator, Expr>(&LhsFactory);
        _rhs = new ValueLazy<CXXRewrittenBinaryOperator, Expr>(&RhsFactory);
    }

    public Expr LHS => _lhs.GetValue(this);

    public static bool IsAssignmentOp => false;

    public static bool IsComparisonOp => true;

    public bool IsReversed => Handle.IsReversed;

    public CXBinaryOperatorKind Opcode => Operator;

    public CXBinaryOperatorKind Operator => Handle.BinaryOperatorKind;

    public string OpcodeStr => Handle.BinaryOperatorKindSpelling.CString;

    public Expr RHS => _rhs.GetValue(this);

    public Expr SemanticForm => (Expr)Children[0];

    private static unsafe Expr RhsFactory(CXXRewrittenBinaryOperator self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.RhsExpr);

    private static unsafe Expr LhsFactory(CXXRewrittenBinaryOperator self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.LhsExpr);
}
