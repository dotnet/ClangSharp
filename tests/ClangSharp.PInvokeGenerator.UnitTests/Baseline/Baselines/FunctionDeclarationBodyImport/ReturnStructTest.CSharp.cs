namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;
    }

    public static partial class Methods
    {
        public static MyStruct MyFunction()
        {
            MyStruct myStruct = new MyStruct();

            return myStruct;
        }
    }
}
