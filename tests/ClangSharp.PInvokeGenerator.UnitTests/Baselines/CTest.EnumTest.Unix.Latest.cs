namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("__AnonymousEnum_ClangUnsavedFile_L8_C5")]
        public uint field;

        public const uint VALUEA = 0;
        public const uint VALUEB = 1;
        public const uint VALUEC = 2;
    }

    public static partial class Methods
    {
        public const uint VALUE1 = 0;
        public const uint VALUE2 = 1;
        public const uint VALUE3 = 2;
    }
}
