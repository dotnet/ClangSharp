namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("MyBuffer")]
        public fixed float c[3];
    }
}
