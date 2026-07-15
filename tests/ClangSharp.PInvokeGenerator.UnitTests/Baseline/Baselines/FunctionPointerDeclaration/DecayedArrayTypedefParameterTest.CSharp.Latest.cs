namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("Callback")]
        public delegate* unmanaged[Cdecl]<ushort*, int> _callback;
    }
}
