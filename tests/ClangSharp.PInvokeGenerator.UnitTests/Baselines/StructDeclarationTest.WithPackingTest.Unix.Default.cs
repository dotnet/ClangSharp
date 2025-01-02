using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Sequential, Pack = CustomPackValue)]
    public partial struct MyStruct
    {
        [NativeTypeName("size_t[2]")]
        public _FixedBuffer_e__FixedBuffer FixedBuffer;

        public partial struct _FixedBuffer_e__FixedBuffer
        {
            public nuint e0;
            public nuint e1;

            public ref nuint this[int index]
            {
                get
                {
                    return ref AsSpan()[index];
                }
            }

            public Span<nuint> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 2);
        }
    }
}
