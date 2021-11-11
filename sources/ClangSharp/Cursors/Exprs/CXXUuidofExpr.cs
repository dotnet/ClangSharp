// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXUuidofExpr : Expr
    {
        private readonly Lazy<Type> _typeOperand;
        private readonly Lazy<MSGuidDecl> _guidDecl;

        internal CXXUuidofExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXUuidofExpr)
        {
            Debug.Assert(NumChildren is 0 or 1);

            _typeOperand = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
            _guidDecl = new Lazy<MSGuidDecl>(() => TranslationUnit.GetOrCreate<MSGuidDecl>(Handle.Referenced));
        }

        public Expr ExprOperand => (Expr)Children.SingleOrDefault();

        public MSGuidDecl GuidDecl => _guidDecl.Value;

        public bool IsTypeOperand => NumChildren is 0;

        public Type TypeOperand => _typeOperand.Value;
    }
}
