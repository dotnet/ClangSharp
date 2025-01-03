using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("float[]")]
        public _x_e__FixedBuffer x;

        public partial struct _x_e__FixedBuffer
        {
            public float e0;

            [UnscopedRef]
            public ref float this[int index]
            {
                get
                {
                    return ref Unsafe.Add(ref e0, index);
                }
            }

            [UnscopedRef]
            public Span<float> AsSpan(int length) => MemoryMarshal.CreateSpan(ref e0, length);
        }
    }
}
