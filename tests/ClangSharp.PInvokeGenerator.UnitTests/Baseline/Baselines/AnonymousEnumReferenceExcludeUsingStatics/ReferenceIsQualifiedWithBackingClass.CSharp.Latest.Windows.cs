namespace ClangSharp.Test
{
    public enum MyEnum2
    {
        MyEnum2_Value1 = Methods.MyEnum1_Value1,
    }

    public static partial class Methods
    {
        public const int MyEnum1_Value1 = 1;
    }
}
