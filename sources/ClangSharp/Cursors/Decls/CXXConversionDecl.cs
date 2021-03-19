// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConversionDecl : CXXMethodDecl
    {
        private readonly Lazy<Type> _conversionType;

        internal CXXConversionDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConversionFunction, CX_DeclKind.CX_DeclKind_CXXConversion)
        {
            _conversionType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public new CXXConversionDecl CanonicalDecl => (CXXConversionDecl)base.CanonicalDecl;

        public bool IsExplicit => !Handle.IsImplicit;

        public Type ConversionType => _conversionType.Value;
    }
}
