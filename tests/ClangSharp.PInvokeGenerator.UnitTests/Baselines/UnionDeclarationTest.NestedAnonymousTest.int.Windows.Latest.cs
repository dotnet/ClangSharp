using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public int r;

        [FieldOffset(0)]
        public int g;

        [FieldOffset(0)]
        public int b;

        [FieldOffset(0)]
        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L11_C5")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref int a
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
        public Span<int> buffer
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
            public int a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName("int[4]")]
            public _buffer_e__FixedBuffer buffer;

            [InlineArray(4)]
            public partial struct _buffer_e__FixedBuffer
            {
                public int e0;
            }
        }
    }
}
