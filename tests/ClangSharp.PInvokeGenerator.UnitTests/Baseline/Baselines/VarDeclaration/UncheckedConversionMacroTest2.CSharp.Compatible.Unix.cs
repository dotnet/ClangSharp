using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro3 MyMacro2(3)")]
        public const int MyMacro3 = unchecked((int)(((nuint)(1) << 31) | ((nuint)(2) << 16) | ((nuint)(3))));
    }
}
