namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0.f")]
        public const float MyMacro1 = 0.0f;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const float MyMacro2 = 0.0f;
    }
}
