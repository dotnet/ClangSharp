using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public double value;
    }

    public unsafe partial struct MyStruct
    {
        public double x;

        public double y;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L10_C5")]
        public _Anonymous_e__Struct Anonymous;

        public ref double z
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.z, 1));
            }
        }

        public ref _Anonymous_e__Struct._w_e__Struct w
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.w, 1));
            }
        }

        public ref double value1
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous1.value1, 1));
            }
        }

        public ref double value
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous1.Anonymous.value, 1));
            }
        }

        public ref double value2
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous2.value2, 1));
            }
        }

        public ref MyUnion u
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.u, 1));
            }
        }

        public Span<double> buffer1
        {
            get
            {
                return MemoryMarshal.CreateSpan(ref Anonymous.buffer1[0], 4);
            }
        }

        public Span<MyUnion> buffer2
        {
            get
            {
                return Anonymous.buffer2.AsSpan();
            }
        }

        public unsafe partial struct _Anonymous_e__Struct
        {
            public double z;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L14_C9")]
            public _w_e__Struct w;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L19_C9")]
            public _Anonymous1_e__Struct Anonymous1;

            [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L29_C9")]
            public _Anonymous2_e__Union Anonymous2;

            public MyUnion u;

            [NativeTypeName("double[4]")]
            public fixed double buffer1[4];

            [NativeTypeName("MyUnion[4]")]
            public _buffer2_e__FixedBuffer buffer2;

            public partial struct _w_e__Struct
            {
                public double value;
            }

            public partial struct _Anonymous1_e__Struct
            {
                public double value1;

                [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L23_C13")]
                public _Anonymous_e__Struct Anonymous;

                public partial struct _Anonymous_e__Struct
                {
                    public double value;
                }
            }

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous2_e__Union
            {
                [FieldOffset(0)]
                public double value2;
            }

            public partial struct _buffer2_e__FixedBuffer
            {
                public MyUnion e0;
                public MyUnion e1;
                public MyUnion e2;
                public MyUnion e3;

                public ref MyUnion this[int index]
                {
                    get
                    {
                        return ref AsSpan()[index];
                    }
                }

                public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 4);
            }
        }
    }
}
