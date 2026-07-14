using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct GpuCaptureUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("struct GpuCaptureParameters")]
        public GpuCaptureParameters GpuCaptureParametersField;

        [FieldOffset(0)]
        public int other;

        public partial struct GpuCaptureParameters
        {
            public int a;

            public int b;
        }
    }
}
