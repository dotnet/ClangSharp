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
            public UIntPtr e0;
            public UIntPtr e1;

            public unsafe ref UIntPtr this[int index]
            {
                get
                {
                    fixed (UIntPtr* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
