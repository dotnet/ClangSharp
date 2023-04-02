// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class CXXConversionDecl : CXXMethodDecl
{
    private readonly Lazy<Type> _conversionType;

    internal CXXConversionDecl(CXCursor handle) : base(handle, CXCursor_ConversionFunction, CX_DeclKind_CXXConversion)
    {
        _conversionType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public new CXXConversionDecl CanonicalDecl => (CXXConversionDecl)base.CanonicalDecl;

    public bool IsExplicit => !Handle.IsImplicit;

    public Type ConversionType => _conversionType.Value;
}
