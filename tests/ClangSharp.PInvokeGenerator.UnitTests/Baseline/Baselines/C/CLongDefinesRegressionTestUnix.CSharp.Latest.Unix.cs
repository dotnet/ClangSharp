namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [return: NativeTypeName("_Bool")]
        public static bool SDL_size_add_check_overflow([NativeTypeName("size_t")] nuint a, [NativeTypeName("size_t")] nuint b, [NativeTypeName("size_t *")] nuint* ret)
        {
            if (b > (18446744073709551615U) - a)
            {
                return (0) != 0;
            }

            *ret = a + b;
            return (1) != 0;
        }

        [NativeTypeName("#define true 1")]
        public const int @true = 1;

        [NativeTypeName("#define false 0")]
        public const int @false = 0;
    }
}
