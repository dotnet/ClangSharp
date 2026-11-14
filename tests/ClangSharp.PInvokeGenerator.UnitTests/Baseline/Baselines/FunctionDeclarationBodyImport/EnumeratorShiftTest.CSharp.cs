using static ClangSharp.Test.MyEnum;

namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Shift = 24,
        MyEnum_Value = 1 << (int)(MyEnum_Shift),
    }

    public static partial class Methods
    {
        [return: NativeTypeName("unsigned char")]
        public static byte MyFunction([NativeTypeName("unsigned int")] uint x)
        {
            return (byte)(x >> (int)(MyEnum_Shift));
        }

        [return: NativeTypeName("unsigned int")]
        public static uint MyFunction2([NativeTypeName("unsigned int")] uint x)
        {
            return x << (int)(MyEnum_Shift);
        }
    }
}
