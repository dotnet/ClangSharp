namespace ClangSharp.Test
{
    public partial struct Something
    {
        [NativeTypeName("int")]
        public SomethingType type;

        public int other;
    }
}
