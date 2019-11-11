// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public partial struct CXSourceLocation
    {
        [NativeTypeName("const void *[2]")]
        public _ptr_data_e__FixedBuffer ptr_data;

        [NativeTypeName("unsigned int")]
        public uint int_data;

        public unsafe partial struct _ptr_data_e__FixedBuffer
        {
            internal void* e0;
            internal void* e1;

            public ref void* this[int index]
            {
                get
                {
                    fixed (void** pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
