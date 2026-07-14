namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro3 MyMacro2(3)")]
        public const int MyMacro3 = unchecked((int)(((uint)(1) << 31) | ((uint)(2) << 16) | ((uint)(3))));
    }
}
