namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0U")]
        public const uint MyMacro1 = 0U;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const uint MyMacro2 = 0U;
    }
}
