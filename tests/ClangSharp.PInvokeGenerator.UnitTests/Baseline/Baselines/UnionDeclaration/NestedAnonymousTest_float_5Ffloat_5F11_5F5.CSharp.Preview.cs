using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public float value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public float r;

        [FieldOffset(0)]
        public float g;

        [FieldOffset(0)]
        public float b;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L11_C5")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref float a
        {
            get
            {
                return ref Anonymous.a;
            }
        }

        [UnscopedRef]
        public ref MyStruct s
        {
            get
            {
                return ref Anonymous.s;
            }
        }

        [UnscopedRef]
        public Span<float> buffer
        {
            get
            {
                return Anonymous.buffer;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public float a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName("float[4]")]
            public _buffer_e__FixedBuffer buffer;

            [InlineArray(4)]
            public partial struct _buffer_e__FixedBuffer
            {
                public float e0;
            }
        }
    }
}
