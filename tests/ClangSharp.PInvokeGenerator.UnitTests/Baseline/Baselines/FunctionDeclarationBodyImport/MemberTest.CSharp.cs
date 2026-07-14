namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public static unsafe partial class Methods
    {
        public static int MyFunction1(MyStruct instance)
        {
            return instance.value;
        }

        public static int MyFunction2(MyStruct* instance)
        {
            return instance->value;
        }
    }
}
