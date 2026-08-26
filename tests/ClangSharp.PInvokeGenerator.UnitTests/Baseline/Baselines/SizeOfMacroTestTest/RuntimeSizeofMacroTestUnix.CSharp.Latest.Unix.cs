namespace ClangSharp.Test
{
    public unsafe partial struct XrEventDataBuffer
    {
        [NativeTypeName("const void *")]
        public void* next;
    }

    public static unsafe partial class Methods
    {
        [NativeTypeName("#define XR_MAX_EVENT_DATA_SIZE sizeof(XrEventDataBuffer)")]
        public static readonly nuint XR_MAX_EVENT_DATA_SIZE = unchecked((nuint)((uint)(sizeof(XrEventDataBuffer))));
    }
}
