// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_AtomicOperatorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class AtomicExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _subExprs;
    private readonly ValueLazy<Type> _valueType;

    internal AtomicExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_AtomicExpr)
    {
        _subExprs = LazyList.Create<Expr, Stmt>(_children);
        _valueType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public CX_AtomicOperatorKind Op => Handle.AtomicOperatorKind;

    public uint NumSubExprs => NumChildren;

    public Expr Order => SubExprs[1];

    public Expr? OrderFail => NumSubExprs > 3 ? SubExprs[3] : null;

    public Expr Ptr => SubExprs[0];

    public Expr? Scope => (Op is (>= CX_AO__opencl_atomic_compare_exchange_strong and <= CX_AO__opencl_atomic_store and not CX_AO__opencl_atomic_init)
                              or (>= CX_AO__hip_atomic_compare_exchange_strong and <= CX_AO__hip_atomic_store)
                              or (>= CX_AO__scoped_atomic_add_fetch and <= CX_AO__scoped_atomic_xor_fetch)) ? SubExprs[(int)(NumSubExprs - 1)] : null;

    public IReadOnlyList<Expr> SubExprs => _subExprs;

    public Expr? Val1
    {
        get
        {
            return (Op is CX_AO__c11_atomic_init or CX_AO__opencl_atomic_init)
                 ? Order
                 : (NumSubExprs > 2) ? SubExprs[2] : null;
        }
    }

    public Expr? Val => Op == CX_AO__atomic_exchange ? OrderFail : (NumSubExprs > 4) ? SubExprs[4] : null;

    public Type ValueType => _valueType.Value;

    public Expr? Weak => (NumSubExprs > 5) ? SubExprs[5] : null;
}
