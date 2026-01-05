// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXTypeidExpr : Expr
{
    private readonly ValueLazy<Type> _typeOperand;

    internal CXXTypeidExpr(CXCursor handle) : base(handle, CXCursor_CXXTypeidExpr, CX_StmtClass_CXXTypeidExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);
        _typeOperand = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.TypeOperand));
    }

    public Expr? ExprOperand => (Expr?)Children.SingleOrDefault();

    public bool IsPotentiallyEvaluated => Handle.IsPotentiallyEvaluated;

    public bool IsTypeOperand => NumChildren is 0;

    public Type TypeOperand => _typeOperand.Value;
}
