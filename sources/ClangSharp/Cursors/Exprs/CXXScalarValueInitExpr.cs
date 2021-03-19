// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXScalarValueInitExpr : Expr
    {
        private readonly Lazy<Type> _typeSourceInfoType;

        internal CXXScalarValueInitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr)
        {
            Debug.Assert(NumChildren is 0);
            _typeSourceInfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public Type TypeSourceInfoType => _typeSourceInfoType.Value;
    }
}
