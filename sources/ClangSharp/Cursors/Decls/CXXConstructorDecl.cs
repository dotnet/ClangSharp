// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConstructorDecl : CXXMethodDecl
    {
        internal CXXConstructorDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Constructor)
        {
        }

        public bool IsConvertingConstructor => Handle.CXXConstructor_IsConvertingConstructor;

        public bool IsCopyConstructor => Handle.CXXConstructor_IsCopyConstructor;

        public bool IsDefaultConstructor => Handle.CXXConstructor_IsDefaultConstructor;

        public bool IsMoveConstructor => Handle.CXXConstructor_IsMoveConstructor;
    }
}
