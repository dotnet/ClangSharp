// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_CastKind;
using static ClangSharp.Interop.CX_ExprDependence;
using static ClangSharp.Interop.CX_ExprValueKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXUnaryOperatorKind;

namespace ClangSharp;

public class Expr : ValueStmt
{
    private static readonly Func<Expr, Expr> s_ignoreImplicitCastsSingleStep = (e) => e is ImplicitCastExpr ice ? ice.SubExpr : e is FullExpr fe ? fe.SubExpr : e;

    private static readonly Func<Expr, Expr> s_ignoreImplicitCastsExtraSingleStep = (e) =>
    {
        var subE = s_ignoreImplicitCastsSingleStep(e);

        return subE != e ? subE : e is MaterializeTemporaryExpr mte ? mte.SubExpr : e is SubstNonTypeTemplateParmExpr nttp ? nttp.Replacement : e;
    };

    private static readonly Func<Expr, Expr> s_ignoreCastsSingleStep = (e) => e switch {
        CastExpr ce => ce.SubExpr,
        FullExpr fe => fe.SubExpr,
        MaterializeTemporaryExpr mte => mte.SubExpr,
        SubstNonTypeTemplateParmExpr nttp => nttp.Replacement,
        _ => e,
    };

    private static readonly Func<Expr, Expr> s_ignoreLValueCastsSingleStep = (e) =>
    {
        if (e is CastExpr ce && ce.CastKind != CX_CK_LValueToRValue)
        {
            return e;
        }

        return s_ignoreCastsSingleStep(e);
    };

    private static readonly Func<Expr, Expr> s_ignoreBaseCastsSingleStep = (e) => e is CastExpr ce && ce.CastKind is CX_CK_DerivedToBase or CX_CK_UncheckedDerivedToBase or CX_CK_NoOp ? ce.SubExpr : e;

    private static readonly Func<Expr, Expr> s_ignoreImplicitSingleStep = (e) =>
    {
        var subE = s_ignoreImplicitCastsSingleStep(e);

        return subE != e ? subE : e is MaterializeTemporaryExpr mte ? mte.SubExpr : e is CXXBindTemporaryExpr bte ? bte.SubExpr : e;
    };

    private static readonly Func<Expr, Expr> s_ignoreImplicitAsWrittenSingleStep = (e) => e is ImplicitCastExpr ice ? ice.SubExprAsWritten : s_ignoreImplicitSingleStep(e);

    private static readonly Func<Expr, Expr> s_ignoreParensSingleStep = (e) =>
    {
        if (e is ParenExpr pe)
        {
            return pe.SubExpr;
        }

        if (e is UnaryOperator uo)
        {
            if (uo.Opcode == CXUnaryOperator_Extension)
            {
                return uo.SubExpr;
            }
        }
        else if (e is GenericSelectionExpr gse)
        {
            if (!gse.IsResultDependent)
            {
                var resultExpr = gse.ResultExpr;
                Debug.Assert(resultExpr is not null);
                return resultExpr!;
            }
        }
        else if (e is ChooseExpr ce)
        {
            if (!ce.IsConditionDependent)
            {
                var chosenSubExpr = ce.ChosenSubExpr;
                Debug.Assert(chosenSubExpr is not null);
                return chosenSubExpr!;
            }
        }

        return e;
    };

    private ValueLazy<Expr, Type> _type;

    private protected unsafe Expr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastExpr or < CX_StmtClass_FirstExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _type = new ValueLazy<Expr, Type>(&TypeFactory);
    }

    public bool ContainsErrors => (Dependence & CX_ED_Error) != 0;

    public bool ContainsUnexpandedParameterPack => (Dependence & CX_ED_UnexpandedPack) != 0;

    public CX_ExprDependence Dependence => Handle.ExprDependence;

    public Expr IgnoreCasts => IgnoreExprNodes(this, s_ignoreCastsSingleStep);

    public Expr IgnoreImpCasts => IgnoreExprNodes(this, s_ignoreImplicitCastsSingleStep);

    public Expr IgnoreImplicit => IgnoreExprNodes(this, s_ignoreImplicitSingleStep);

    public Expr IgnoreImplicitAsWritten => IgnoreExprNodes(this, s_ignoreImplicitAsWrittenSingleStep);

    public Expr IgnoreParens => IgnoreExprNodes(this, s_ignoreParensSingleStep);

    public Expr IgnoreParenBaseCasts => IgnoreExprNodes(this, s_ignoreParensSingleStep, s_ignoreBaseCastsSingleStep);

    public Expr IgnoreParenCasts => IgnoreExprNodes(this, s_ignoreParensSingleStep, s_ignoreCastsSingleStep);

    public Expr IgnoreParenImpCasts => IgnoreExprNodes(this, s_ignoreParensSingleStep, s_ignoreImplicitCastsExtraSingleStep);

    public Expr IgnoreParenLValueCasts => IgnoreExprNodes(this, s_ignoreParensSingleStep, s_ignoreLValueCastsSingleStep);

    public bool IsGLValue => ValueKind is CX_VK_LValue or CX_VK_XValue;

    public bool IsImplicitCXXThis
    {
        get
        {
            var e = this;

            while (true)
            {
                if (e is ParenExpr paren)
                {
                    e = paren.SubExpr;
                    continue;
                }

                if (e is ImplicitCastExpr ice)
                {
                    if (ice.CastKind is CX_CK_NoOp or CX_CK_LValueToRValue or CX_CK_DerivedToBase or CX_CK_UncheckedDerivedToBase)
                    {
                        e = ice.SubExpr;
                        continue;
                    }
                }

                if (e is UnaryOperator unOp)
                {
                    if (unOp.Opcode == CXUnaryOperator_Extension)
                    {
                        e = unOp.SubExpr;
                        continue;
                    }
                }

                if (e is MaterializeTemporaryExpr m)
                {
                    e = m.SubExpr;
                    continue;
                }

                break;
            }

            return e is CXXThisExpr self && self.IsImplicit;
        }
    }

    public bool IsInstantiationDependent => (Dependence & CX_ED_Instantiation) != 0;

    public bool IsLValue => ValueKind == CX_VK_LValue;

    public bool IsPRValue => ValueKind == CX_VK_PRValue;

    public bool IsTypeDependent => (Dependence & CX_ED_Type) != 0;

    public bool IsValueDependent => (Dependence & CX_ED_Value) != 0;

    public bool IsXValue => ValueKind == CX_VK_XValue;

    public CX_ExprObjectKind ObjectKind => Handle.ObjectKind;

    public Type Type => _type.GetValue(this);

    public CX_ExprValueKind ValueKind => Handle.ValueKind;

    private static Expr IgnoreExprNodes(Expr e, Func<Expr, Expr> fn)
    {
        Expr? lastE = null;

        while (e != lastE)
        {
            lastE = e;
            e = fn(e);
        }

        return e;
    }

    private static Expr IgnoreExprNodes(Expr e, Func<Expr, Expr> fn1, Func<Expr, Expr> fn2)
    {
        Expr? lastE = null;

        while (e != lastE)
        {
            lastE = e;
            e = fn2(fn1(e));
        }

        return e;
    }

    protected static Expr SkipImplicitTemporary(Expr e)
    {
        // Skip through reference binding to temporary.
        if (e is MaterializeTemporaryExpr materialize)
        {
            e = materialize.SubExpr;
        }

        // Skip any temporary bindings; they're implicit.
        if (e is CXXBindTemporaryExpr binder)
        {
            e = binder.SubExpr;
        }

        return e;
    }

    private static unsafe Type TypeFactory(Expr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.Type);
}
