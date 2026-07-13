namespace ClangSharp.Test
{
    public partial struct MyClass
    {
        [NativeTypeName("const int")]
        private const int x = 1_024;
    }
}
