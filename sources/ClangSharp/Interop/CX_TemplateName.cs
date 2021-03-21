// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

namespace ClangSharp.Interop
{
    public unsafe partial struct CX_TemplateName
    {
        public CX_TemplateNameKind kind;

        [NativeTypeName("const void *")]
        public void* value;

        [NativeTypeName("CXTranslationUnit")]
        public CXTranslationUnitImpl* tu;
    }
}
