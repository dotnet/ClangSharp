namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public MyStruct(int value)
        {
            this.value = value;
        }

        public MyStruct Add(MyStruct rhs)
        {
            return new MyStruct(value + rhs.value);
        }
    }

    public static partial class Methods
    {
        public static MyStruct MyFunction1(MyStruct lhs, MyStruct rhs)
        {
            return lhs.Add(rhs);
        }

        public static MyStruct Subtract(MyStruct lhs, MyStruct rhs)
        {
            return new MyStruct(lhs.value - rhs.value);
        }

        public static MyStruct MyFunction2(MyStruct lhs, MyStruct rhs)
        {
            return Subtract(lhs, rhs);
        }
    }
}
