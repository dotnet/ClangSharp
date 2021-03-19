// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXMemberCallExpr : CallExpr
    {
        internal CXXMemberCallExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXMemberCallExpr)
        {
        }

        public Expr ImplicitObjectArgument
        {
            get
            {
                Expr callee = Callee.IgnoreParens;

                if (callee is MemberExpr MemExpr)
                {
                    return MemExpr.Base;
                }

                if (callee is BinaryOperator BO)
                {
                    if (BO.IsPtrMemOp)
                    {
                        return BO.LHS;
                    }
                }

                return null;
            }
        }

        public CXXMethodDecl MethodDecl
        {
            get
            {
                if (Callee.IgnoreParens is MemberExpr MemExpr)
                {
                    return (CXXMethodDecl)MemExpr.MemberDecl;
                }

                return null;
            }
        }

        public Type ObjectType
        {
            get
            {
                Type Ty = ImplicitObjectArgument.Type;

                if (Ty.IsPointerType)
                {
                    Ty = Ty.PointeeType;
                }
                return Ty;
            }
        }

        public CXXRecordDecl RecordDecl
        {
            get
            {
                Expr ThisArg = ImplicitObjectArgument;

                if (ThisArg is null)
                {
                    return null;
                }

                if (ThisArg.Type.IsAnyPointerType)
                {
                    return ThisArg.Type.PointeeType.AsCXXRecordDecl;
                }

                return ThisArg.Type.AsCXXRecordDecl;
            }
        }
    }
}
