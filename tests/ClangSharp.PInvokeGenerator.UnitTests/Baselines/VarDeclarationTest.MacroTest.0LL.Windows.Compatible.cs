namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0LL")]
        public const long MyMacro1 = 0L;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const long MyMacro2 = 0L;
    }
}
