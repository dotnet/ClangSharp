// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public unsafe partial struct CX_TemplateArgument
    {
        public CXTemplateArgumentKind kind;

        public int xdata;

        [NativeTypeName("const clang::TemplateArgument *")]
        public void* value;

        [NativeTypeName("CXTranslationUnit")]
        public CXTranslationUnitImpl* tu;
    }
}
