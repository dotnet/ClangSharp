namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 unsigned(-1)")]
        public const uint MyMacro1 = unchecked((uint)(-1));
    }
}
