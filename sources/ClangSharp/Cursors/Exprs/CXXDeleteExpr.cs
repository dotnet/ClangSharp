// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDeleteExpr : Expr
    {
        private readonly Lazy<Type> _destroyedType;
        private readonly Lazy<FunctionDecl> _operatorDelete;

        internal CXXDeleteExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXDeleteExpr, CX_StmtClass.CX_StmtClass_CXXDeleteExpr)
        {
            Debug.Assert(NumChildren is 1);

            _destroyedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
            _operatorDelete = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Referenced));
        }

        public Expr Argument => (Expr)Children[0];

        public Type DestroyedType => _destroyedType.Value;

        public bool DoesUsualArrayDeleteWantSize => Handle.DoesUsualArrayDeleteWantSize;

        public bool IsArrayForm => Handle.IsArrayForm;

        public bool IsArrayFormAsWritten => Handle.IsArrayFormAsWritten;

        public bool IsGlobalDelete => Handle.IsGlobal;

        public FunctionDecl OperatorDelete => _operatorDelete.Value;
    }
}
