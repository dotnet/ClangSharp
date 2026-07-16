namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [NativeTypeName("#define MY_NULL_HANDLE nullptr")]
        public static void* MY_NULL_HANDLE => null;
    }
}
