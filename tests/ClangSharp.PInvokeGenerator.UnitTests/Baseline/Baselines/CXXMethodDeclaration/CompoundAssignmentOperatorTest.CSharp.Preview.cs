namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int value;

        public void operator +=(MyStruct rhs)
        {
            value += rhs.value;
        }

        public void operator *=(int scale)
        {
            value *= scale;
        }

        [return: NativeTypeName("MyStruct &")]
        public MyStruct* op_DivisionAssignment([NativeTypeName("MyStruct &")] MyStruct* other)
        {
            value /= other->value;
            return other;
        }
    }
}
