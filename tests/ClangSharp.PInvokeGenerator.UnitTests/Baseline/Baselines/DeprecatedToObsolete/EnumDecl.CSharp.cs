using System;

namespace ClangSharp.Test
{
    public enum MyEnum0
    {
        MyEnum_Value0,
    }

    [Obsolete]
    public enum MyEnum1
    {
        MyEnum_Value1,
    }

    [Obsolete("This is obsolete.")]
    public enum MyEnum2
    {
        MyEnum_Value2,
    }

    public enum MyEnum3
    {
        MyEnum_Value3,
    }
}
