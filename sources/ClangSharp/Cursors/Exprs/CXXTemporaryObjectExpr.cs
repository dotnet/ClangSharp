// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXTemporaryObjectExpr : CXXConstructExpr
    {
        private readonly Lazy<Type> _typeSourceInfoType;

        internal CXXTemporaryObjectExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr)
        {
            _typeSourceInfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public Type TypeSourceInfoType => _typeSourceInfoType.Value;
    }
}
