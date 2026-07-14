using System;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define MyMacro1 (long)0x80000000L")]
        public static readonly IntPtr MyMacro1 = unchecked((nint)(0x80000000));

        [NativeTypeName("#define MyMacro2 (int)0x80000000")]
        public const int MyMacro2 = unchecked((int)(0x80000000));
    }
}
