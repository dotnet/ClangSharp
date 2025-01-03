namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("MyBuffer")]
        public fixed ulong c[3];
    }
}
