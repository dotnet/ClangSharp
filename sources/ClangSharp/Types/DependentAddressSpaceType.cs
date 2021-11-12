// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentAddressSpaceType : Type
    {
        private readonly Lazy<Expr> _addrSpaceExpr;

        internal DependentAddressSpaceType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentAddressSpace)
        {
            _addrSpaceExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.AddrSpaceExpr));
        }

        public Expr AddrSpaceExpr => _addrSpaceExpr.Value;
    }
}
