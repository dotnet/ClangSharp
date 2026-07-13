namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 0 + \\\n1")]
        public const int MyMacro1 = 0 + 1;
    }
}
