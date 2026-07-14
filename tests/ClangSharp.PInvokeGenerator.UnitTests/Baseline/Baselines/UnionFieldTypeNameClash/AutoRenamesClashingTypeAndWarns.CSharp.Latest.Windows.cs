using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct GpuCaptureUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("struct GpuCaptureParameters")]
        public _GpuCaptureParameters_e__Struct GpuCaptureParameters;

        [FieldOffset(0)]
        public int other;

        public partial struct _GpuCaptureParameters_e__Struct
        {
            public int a;

            public int b;
        }
    }
}
