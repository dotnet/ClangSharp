// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ExplicitCastExpr : CastExpr
    {
        private readonly Lazy<Type> _typeAsWritten;

        private protected ExplicitCastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastExplicitCastExpr or < CX_StmtClass.CX_StmtClass_FirstExplicitCastExpr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _typeAsWritten = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public Type TypeAsWritten => _typeAsWritten.Value;
    }
}
