using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct _MyUnion
    {
        [FieldOffset(0)]
        public int _field;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _MyOtherUnion
    {
        [FieldOffset(0)]
        [NativeTypeName("MyUnion")]
        public _MyUnion _field1;

        [FieldOffset(0)]
        [NativeTypeName("MyUnion *")]
        public _MyUnion* _field2;
    }
}
