using static ClangSharp.Test.MyEnum;

namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public static partial class Methods
    {
        public static int MyFunction(MyEnum x)
        {
            return (x == MyEnum_Value0 || x == MyEnum_Value1 || x == MyEnum_Value2) ? 1 : 0;
        }
    }
}
