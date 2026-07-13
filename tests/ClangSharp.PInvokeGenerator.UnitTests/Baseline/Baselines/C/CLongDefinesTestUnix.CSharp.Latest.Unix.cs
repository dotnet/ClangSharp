namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define SIZE_MAX (18446744073709551615UL)")]
        public static readonly nuint SIZE_MAX = unchecked((nuint)(18446744073709551615U));

        [NativeTypeName("#define CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM SIZE_MAX")]
        public static readonly nuint CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM = unchecked((nuint)(18446744073709551615U));

        [NativeTypeName("#define LONG_MAX __LONG_MAX__")]
        public static readonly nint LONG_MAX = unchecked((nint)(9223372036854775807));

        [NativeTypeName("#define ULONG_MAX (__LONG_MAX__ *2UL+1UL)")]
        public static readonly nuint ULONG_MAX = unchecked((nuint)(9223372036854775807 * 2U + 1U));

        [NativeTypeName("#define CONST_IN_RANGE_S1 ((long)2147483647)")]
        public const nint CONST_IN_RANGE_S1 = ((nint)(2147483647));

        [NativeTypeName("#define CONST_IN_RANGE_U1 ((unsigned long)4294967295)")]
        public const nuint CONST_IN_RANGE_U1 = ((nuint)(4294967295));

        [NativeTypeName("#define READONLY_OUT_OF_RANGE_S1 ((long)4294967295)")]
        public static readonly nint READONLY_OUT_OF_RANGE_S1 = unchecked((nint)(4294967295));

        [NativeTypeName("#define READONLY_OUT_OF_RANGE_S2 ((long)4294967296)")]
        public static readonly nint READONLY_OUT_OF_RANGE_S2 = unchecked((nint)(4294967296));

        [NativeTypeName("#define READONLY_OUT_OF_RANGE_U1 ((unsigned long)4294967296)")]
        public static readonly nuint READONLY_OUT_OF_RANGE_U1 = unchecked((nuint)(4294967296));
    }
}
