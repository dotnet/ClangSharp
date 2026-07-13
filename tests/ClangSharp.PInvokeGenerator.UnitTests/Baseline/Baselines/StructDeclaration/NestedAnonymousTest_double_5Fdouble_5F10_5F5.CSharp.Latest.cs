using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public double value;
    }

    public partial struct MyStruct
    {
        public double x;

        public double y;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L10_C5")]
        public _Anonymous_e__Struct Anonymous;

        [UnscopedRef]
        public ref double z
        {
            get
            {
                return ref Anonymous.z;
            }
        }

        [UnscopedRef]
        public ref _Anonymous_e__Struct._w_e__Struct w
        {
            get
            {
                return ref Anonymous.w;
            }
        }

        [UnscopedRef]
        public ref double value1
        {
            get
            {
                return ref Anonymous.Anonymous1.value1;
            }
        }

        [UnscopedRef]
        public ref double value
        {
            get
            {
                return ref Anonymous.Anonymous1.Anonymous.value;
            }
        }

        [UnscopedRef]
        public ref double value2
        {
            get
            {
                return ref Anonymous.Anonymous2.value2;
            }
        }

        [UnscopedRef]
        public ref MyUnion u
        {
            get
            {
                return ref Anonymous.u;
            }
        }

        [UnscopedRef]
        public Span<double> buffer1
        {
            get
            {
                return Anonymous.buffer1;
            }
        }

        [UnscopedRef]
        public Span<MyUnion> buffer2
        {
            get
            {
                return Anonymous.buffer2;
            }
        }

        public partial struct _Anonymous_e__Struct
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
            public _buffer1_e__FixedBuffer buffer1;

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

            [InlineArray(4)]
            public partial struct _buffer1_e__FixedBuffer
            {
                public double e0;
            }

            [InlineArray(4)]
            public partial struct _buffer2_e__FixedBuffer
            {
                public MyUnion e0;
            }
        }
    }
}
