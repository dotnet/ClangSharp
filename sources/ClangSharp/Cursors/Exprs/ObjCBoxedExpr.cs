// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCBoxedExpr : Expr
    {
        private readonly Lazy<ObjCMethodDecl> _boxingMethod;

        internal ObjCBoxedExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCBoxedExpr)
        {
            Debug.Assert(NumChildren is 1);
            _boxingMethod = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        }

        public ObjCMethodDecl BoxingMethod => _boxingMethod.Value;

        public Expr SubExpr => (Expr)Children[0];
    }
}
