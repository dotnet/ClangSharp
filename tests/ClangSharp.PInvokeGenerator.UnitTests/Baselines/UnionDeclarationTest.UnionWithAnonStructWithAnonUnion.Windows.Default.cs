using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _MY_UNION
    {
        [FieldOffset(0)]
        [NativeTypeName("long[2]")]
        public fixed int AsArray[2];

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L4_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref int First
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.First, 1));
            }
        }

        public ref _Anonymous_e__Struct._Anonymous_e__Union._A_e__Struct A
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.A, 1));
            }
        }

        public ref _Anonymous_e__Struct._Anonymous_e__Union._B_e__Struct B
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.B, 1));
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            [NativeTypeName("long")]
            public int First;

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
                    public int Second;
                }

                public partial struct _B_e__Struct
                {
                    [NativeTypeName("long")]
                    public int Second;
                }
            }
        }
    }
}
