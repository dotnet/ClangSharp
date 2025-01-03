namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName("MyStruct[3]")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public unsafe ref MyStruct this[int index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
