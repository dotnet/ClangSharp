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
        public static MyStruct Subtract(MyStruct lhs, MyStruct rhs)
        {
            return new MyStruct(lhs.value - rhs.value);
        }
    }
}
