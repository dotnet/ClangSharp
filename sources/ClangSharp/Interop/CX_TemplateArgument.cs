// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public unsafe partial struct CX_TemplateArgument
    {
        public CXTemplateArgumentKind kind;

        public int xdata;

        public _Anonymous_e__Union Anonymous;

        [NativeTypeName("const clang::TemplateArgument*")]
        public void* value;

        public CXTranslationUnit tu;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public CXCursor parentCursor;

            [FieldOffset(0)]
            public CXType parentType;
        }
    }
}
