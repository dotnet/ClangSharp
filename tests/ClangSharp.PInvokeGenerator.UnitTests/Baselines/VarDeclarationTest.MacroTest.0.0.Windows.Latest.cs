namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0.0")]
        public const double MyMacro1 = 0.0;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const double MyMacro2 = 0.0;
    }
}
