namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName("#define SIZE_MAX 0xffffffffffffffffui64")]
        public const ulong SIZE_MAX = 0xffffffffffffffffUL;

        [NativeTypeName("#define CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM SIZE_MAX")]
        public const ulong CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM = 0xffffffffffffffffUL;

        [NativeTypeName("#define LONG_MAX 2147483647L")]
        public const int LONG_MAX = 2147483647;

        [NativeTypeName("#define ULONG_MAX 0xffffffffUL")]
        public const uint ULONG_MAX = 0xffffffffU;
    }
}
