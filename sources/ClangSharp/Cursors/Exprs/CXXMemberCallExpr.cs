// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
                var callee = Callee.IgnoreParens;

                if (callee is MemberExpr memExpr)
                {
                    return memExpr.Base;
                }

                if (callee is BinaryOperator bo)
                {
                    if (bo.IsPtrMemOp)
                    {
                        return bo.LHS;
                    }
                }

                return null;
            }
        }

        public CXXMethodDecl MethodDecl => Callee.IgnoreParens is MemberExpr memExpr ? (CXXMethodDecl)memExpr.MemberDecl : null;

        public Type ObjectType
        {
            get
            {
                var ty = ImplicitObjectArgument.Type;

                if (ty.IsPointerType)
                {
                    ty = ty.PointeeType;
                }
                return ty;
            }
        }

        public CXXRecordDecl RecordDecl
        {
            get
            {
                var thisArg = ImplicitObjectArgument;

                return thisArg is null
                    ? null
                    : thisArg.Type.IsAnyPointerType ? thisArg.Type.PointeeType.AsCXXRecordDecl : thisArg.Type.AsCXXRecordDecl;
            }
        }
    }
}
