namespace ClangSharp.Test
{
    public partial struct MyClass
    {
        [NativeTypeName("const float")]
        private const float x = 1_024;
    }
}
