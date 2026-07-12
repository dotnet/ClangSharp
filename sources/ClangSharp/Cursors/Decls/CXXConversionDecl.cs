// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXConversionDecl : CXXMethodDecl
{
    private ValueLazy<CXXConversionDecl, Type> _conversionType;

    internal unsafe CXXConversionDecl(CXCursor handle) : base(handle, CXCursor_ConversionFunction, CX_DeclKind_CXXConversion)
    {
        _conversionType = new ValueLazy<CXXConversionDecl, Type>(&ConversionTypeFactory);
    }

    public new CXXConversionDecl CanonicalDecl => (CXXConversionDecl)base.CanonicalDecl;

    public bool IsExplicit => !Handle.IsImplicit;

    public Type ConversionType => _conversionType.GetValue(this);

    private static unsafe Type ConversionTypeFactory(CXXConversionDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
