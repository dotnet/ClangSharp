// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentAddressSpaceType : Type
{
    private ValueLazy<DependentAddressSpaceType, Expr> _addrSpaceExpr;

    internal unsafe DependentAddressSpaceType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentAddressSpace)
    {
        _addrSpaceExpr = new ValueLazy<DependentAddressSpaceType, Expr>(&AddrSpaceExprFactory);
    }

    public Expr AddrSpaceExpr => _addrSpaceExpr.GetValue(this);

    private static unsafe Expr AddrSpaceExprFactory(DependentAddressSpaceType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.AddrSpaceExpr);
}
