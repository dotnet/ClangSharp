// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConversionDecl : CXXMethodDecl
    {
        internal CXXConversionDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConversionFunction, CX_DeclKind.CX_DeclKind_CXXConversion)
        {
        }

        public new CXXConversionDecl CanonicalDecl => (CXXConversionDecl)base.CanonicalDecl;
    }
}
