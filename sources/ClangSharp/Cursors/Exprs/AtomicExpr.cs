// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class AtomicExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _subExprs;
    private readonly Lazy<Type> _valueType;

    internal AtomicExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_AtomicExpr)
    {
        _subExprs = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        _valueType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public CX_AtomicOperatorKind Op => Handle.AtomicOperatorKind;

    public uint NumSubExprs => NumChildren;

    public Expr Order => SubExprs[1];

    public Expr OrderFail => NumSubExprs > 3 ? SubExprs[3] : null;

    public Expr Ptr => SubExprs[0];

    public Expr Scope => (Op is >= CX_AtomicOperatorKind.CX_AO__opencl_atomic_load and <= CX_AtomicOperatorKind.CX_AO__opencl_atomic_fetch_max) ? SubExprs[(int)(NumSubExprs - 1)] : null;

    public IReadOnlyList<Expr> SubExprs => _subExprs.Value;

    public Expr Val1
    {
        get
        {
            return Op is CX_AtomicOperatorKind.CX_AO__c11_atomic_init or CX_AtomicOperatorKind.CX_AO__opencl_atomic_init
                ? Order
                : (NumSubExprs > 2) ? SubExprs[2] : null;
        }
    }

    public Expr Val => Op == CX_AtomicOperatorKind.CX_AO__atomic_exchange ? OrderFail : (NumSubExprs > 4) ? SubExprs[4] : null;

    public Type ValueType => _valueType.Value;

    public Expr Weak => (NumSubExprs > 5) ? SubExprs[5] : null;
}
