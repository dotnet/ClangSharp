namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("signed char")]
        public sbyte r;

        [NativeTypeName("signed char")]
        public sbyte g;

        [NativeTypeName("signed char")]
        public sbyte b;
    }
}
