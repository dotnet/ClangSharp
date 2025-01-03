namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("float[]")]
        public fixed float x[1];
    }
}
