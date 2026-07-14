using System;

namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        [Obsolete]
        MyEnum_Value1,
        [Obsolete("This is obsolete.")]
        MyEnum_Value2,
        MyEnum_Value3,
    }
}
