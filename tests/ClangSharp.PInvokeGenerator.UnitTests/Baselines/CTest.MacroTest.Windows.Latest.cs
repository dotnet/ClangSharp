namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 5")]
        public const int MyMacro1 = 5;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const int MyMacro2 = 5;
    }
}
