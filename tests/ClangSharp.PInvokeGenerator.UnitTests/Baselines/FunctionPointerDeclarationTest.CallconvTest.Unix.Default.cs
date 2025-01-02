namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("Callback")]
        public delegate* unmanaged[Stdcall]<void> _callback;
    }
}
