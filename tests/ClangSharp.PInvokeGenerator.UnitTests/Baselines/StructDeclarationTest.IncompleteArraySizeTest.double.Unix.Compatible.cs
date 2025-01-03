namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [NativeTypeName("double[]")]
        public fixed double x[1];
    }
}
