namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public float x;

        public float y;

        public float z;

        public float w;
    }

    public static partial class Methods
    {
        public static MyStruct MyFunction1()
        {
            return new MyStruct
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
                w = 4.0f,
            };
        }

        public static MyStruct MyFunction2()
        {
            return new MyStruct
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
            };
        }
    }
}
