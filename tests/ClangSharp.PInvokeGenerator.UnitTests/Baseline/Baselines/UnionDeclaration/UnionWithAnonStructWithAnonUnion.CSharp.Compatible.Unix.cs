using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _MY_UNION
    {
        [FieldOffset(0)]
        [NativeTypeName("long[2]")]
        public _AsArray_e__FixedBuffer AsArray;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L4_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref IntPtr First
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->First;
                }
            }
        }

        public ref _Anonymous_e__Struct._Anonymous_e__Union._A_e__Struct A
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous_e__Union* pField = &Anonymous.Anonymous)
                {
                    return ref pField->A;
                }
            }
        }

        public ref _Anonymous_e__Struct._Anonymous_e__Union._B_e__Struct B
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous_e__Union* pField = &Anonymous.Anonymous)
                {
                    return ref pField->B;
                }
            }
        }

        public unsafe partial struct _Anonymous_e__Struct
        {
            [NativeTypeName("long")]
            public IntPtr First;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L7_C9")]
            public _Anonymous_e__Union Anonymous;

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous_e__Union
            {
                [FieldOffset(0)]
                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L9_C13")]
                public _A_e__Struct A;

                [FieldOffset(0)]
                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L14_C13")]
                public _B_e__Struct B;

                public partial struct _A_e__Struct
                {
                    [NativeTypeName("long")]
                    public IntPtr Second;
                }

                public partial struct _B_e__Struct
                {
                    [NativeTypeName("long")]
                    public IntPtr Second;
                }
            }
        }

        public partial struct _AsArray_e__FixedBuffer
        {
            public IntPtr e0;
            public IntPtr e1;

            public unsafe ref IntPtr this[int index]
            {
                get
                {
                    fixed (IntPtr* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
