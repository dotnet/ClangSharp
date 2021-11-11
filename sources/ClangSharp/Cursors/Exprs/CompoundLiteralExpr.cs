// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
