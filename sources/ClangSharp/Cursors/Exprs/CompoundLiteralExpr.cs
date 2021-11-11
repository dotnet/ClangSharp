// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundLiteralExpr : Expr
    {
        private readonly Lazy<Type> _typeSourceinfoType;

        internal CompoundLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundLiteralExpr, CX_StmtClass.CX_StmtClass_CompoundLiteralExpr)
        {
            Debug.Assert(NumChildren is 1);
            _typeSourceinfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public bool IsFileScope => Handle.IsFileScope;

        public Expr Initializer => (Expr)Children[0];

        public Type TypeSourceInfoType => _typeSourceinfoType.Value;
    }
}
