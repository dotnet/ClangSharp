using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int value;

        [return: NativeTypeName("MyStruct &")]
        public MyStruct* op_AdditionAssignment(MyStruct rhs)
        {
            value += rhs.value;
            return (MyStruct*)Unsafe.AsPointer(ref this);
        }

        [return: NativeTypeName("MyStruct &")]
        public MyStruct* op_MultiplicationAssignment(int scale)
        {
            value *= scale;
            return (MyStruct*)Unsafe.AsPointer(ref this);
        }

        [return: NativeTypeName("MyStruct &")]
        public MyStruct* op_DivisionAssignment([NativeTypeName("MyStruct &")] MyStruct* other)
        {
            value /= other->value;
            return other;
        }
    }
}
