namespace ClangSharp.Test
{
    public partial struct MyStruct1A
    {
        public int x;

        public int y;
    }

    public partial struct MyStruct1B
    {
        public int x;

        public int y;
    }

    [NativeTypeName("struct MyStruct2 : MyStruct1A, MyStruct1B")]
    [NativeInheritance("MyStruct1B")]
    public partial struct MyStruct2
    {
        public MyStruct1A Base1;

        public MyStruct1B Base2;

        public int z;

        public int w;
    }
}
