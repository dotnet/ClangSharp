// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Expr : ValueStmt
    {
        private static readonly Func<Expr, Expr> IgnoreImplicitCastsSingleStep = (E) =>
        {
            if (E is ImplicitCastExpr ICE)
            {
                return ICE.SubExpr;
            }

            if (E is FullExpr FE)
            {
                return FE.SubExpr;
            }

            return E;
        };

        private static readonly Func<Expr, Expr> IgnoreImplicitSingleStep = (E) =>
        {
            Expr SubE = IgnoreImplicitCastsSingleStep(E);

            if (SubE != E)
            {
                return SubE;
            }

            if (E is MaterializeTemporaryExpr MTE)
            {
                return MTE.SubExpr;
            }

            if (E is CXXBindTemporaryExpr BTE)
            {
                return BTE.SubExpr;
            }

            return E;
        };

        private static readonly Func<Expr, Expr> IgnoreParensSingleStep = (E) =>
        {
            if (E is ParenExpr PE)
            {
                return PE.SubExpr;
            }

            if (E is UnaryOperator UO)
            {
                if (UO.Opcode == CX_UnaryOperatorKind.CX_UO_Extension)
                {
                    return UO.SubExpr;
                }
            }
            else if (E is GenericSelectionExpr GSE)
            {
                if (!GSE.IsResultDependent)
                {
                    return GSE.ResultExpr;
                }
            }
            else if (E is ChooseExpr CE)
            {
                if (!CE.IsConditionDependent)
                {
                    return CE.ChosenSubExpr;
                }
            }

            return E;
        };

        private readonly Lazy<Type> _type;

        private protected Expr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public bool ContainsErrors => (Dependence & CX_ExprDependence.CX_ED_Error) != 0;

        public bool ContainsUnexpandedParameterPack => (Dependence & CX_ExprDependence.CX_ED_UnexpandedPack) != 0;

        public CX_ExprDependence Dependence => Handle.ExprDependence;

        public Expr IgnoreImplicit => IgnoreExprNodes(this, IgnoreImplicitSingleStep);

        public Expr IgnoreParens => IgnoreExprNodes(this, IgnoreParensSingleStep);

        public bool IsImplicitCXXThis
        {
            get
            {
                Expr E = this;

                while (true)
                {
                    if (E is ParenExpr Paren)
                    {
                        E = Paren.SubExpr;
                        continue;
                    }

                    if (E is ImplicitCastExpr ICE)
                    {
                        if ((ICE.CastKind == CX_CastKind.CX_CK_NoOp) || (ICE.CastKind == CX_CastKind.CX_CK_LValueToRValue) || (ICE.CastKind == CX_CastKind.CX_CK_DerivedToBase) || (ICE.CastKind == CX_CastKind.CX_CK_UncheckedDerivedToBase))
                        {
                            E = ICE.SubExpr;
                            continue;
                        }
                    }

                    if (E is UnaryOperator UnOp)
                    {
                        if (UnOp.Opcode == CX_UnaryOperatorKind.CX_UO_Extension)
                        {
                            E = UnOp.SubExpr;
                            continue;
                        }
                    }

                    if (E is MaterializeTemporaryExpr M)
                    {
                        E = M.SubExpr;
                        continue;
                    }

                    break;
                }

                if (E is CXXThisExpr This)
                {
                    return This.IsImplicit;
                }

                return false;
            }
        }

        public bool IsInstantiationDependent => (Dependence & CX_ExprDependence.CX_ED_Instantiation) != 0;

        public bool IsTypeDependent => (Dependence & CX_ExprDependence.CX_ED_Type) != 0;

        public bool IsValueDependent => (Dependence & CX_ExprDependence.CX_ED_Value) != 0;

        public Type Type => _type.Value;

        private static Expr IgnoreExprNodes(Expr E, Func<Expr, Expr> Fn)
        {
            Expr LastE = null;

            while (E != LastE)
            {
                LastE = E;
                E = Fn(E);
            }

            return E;
        }

        protected static Expr SkipImplicitTemporary(Expr E)
        {
            // Skip through reference binding to temporary.
            if (E is MaterializeTemporaryExpr Materialize)
            {
                E = Materialize.SubExpr;
            }

            // Skip any temporary bindings; they're implicit.
            if (E is CXXBindTemporaryExpr Binder)
            {
                E = Binder.SubExpr;
            }

            return E;
        }
    }
}
