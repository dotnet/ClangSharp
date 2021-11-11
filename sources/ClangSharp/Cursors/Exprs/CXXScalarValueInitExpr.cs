// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
