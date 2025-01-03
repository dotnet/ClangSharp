namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName("MyStruct[2][1][3][4]")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0_0_0_0;
            public MyStruct e1_0_0_0;

            public MyStruct e0_0_1_0;
            public MyStruct e1_0_1_0;

            public MyStruct e0_0_2_0;
            public MyStruct e1_0_2_0;

            public MyStruct e0_0_0_1;
            public MyStruct e1_0_0_1;

            public MyStruct e0_0_1_1;
            public MyStruct e1_0_1_1;

            public MyStruct e0_0_2_1;
            public MyStruct e1_0_2_1;

            public MyStruct e0_0_0_2;
            public MyStruct e1_0_0_2;

            public MyStruct e0_0_1_2;
            public MyStruct e1_0_1_2;

            public MyStruct e0_0_2_2;
            public MyStruct e1_0_2_2;

            public MyStruct e0_0_0_3;
            public MyStruct e1_0_0_3;

            public MyStruct e0_0_1_3;
            public MyStruct e1_0_1_3;

            public MyStruct e0_0_2_3;
            public MyStruct e1_0_2_3;

            public unsafe ref MyStruct this[int index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0_0_0_0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
