// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
