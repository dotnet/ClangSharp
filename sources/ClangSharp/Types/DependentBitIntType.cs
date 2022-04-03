// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentBitIntType : Type
    {
        private readonly Lazy<Expr> _numBitsExpr;

        internal DependentBitIntType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentBitInt)
        {
            _numBitsExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.NumBitsExpr));
        }

        public bool IsSigned => Handle.IsSigned;

        public bool IsUnsigned => Handle.IsUnsigned;

        public Expr NumBitsExpr => _numBitsExpr.Value;
    }
}
