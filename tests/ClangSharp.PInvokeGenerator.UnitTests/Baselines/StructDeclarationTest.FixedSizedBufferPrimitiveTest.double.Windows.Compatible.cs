namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("double[3]")]
        public fixed double c[3];
    }
}
