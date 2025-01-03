namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0LLU")]
        public const ulong MyMacro1 = 0UL;

        [NativeTypeName("#define MyMacro2 MyMacro1")]
        public const ulong MyMacro2 = 0UL;
    }
}
