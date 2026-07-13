namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("__AnonymousEnum_ClangUnsavedFile_L8_C5")]
        public int field;

        public const int VALUEA = 0;
        public const int VALUEB = 1;
        public const int VALUEC = 2;
    }

    public static partial class Methods
    {
        public const int VALUE1 = 0;
        public const int VALUE2 = 1;
        public const int VALUE3 = 2;
    }
}
