namespace ClangSharp.Test
{
    public partial struct A
    {

        public partial struct Inner
        {
            public int value;
        }
    }

    public partial struct B
    {
        [NativeTypeName("A::Inner")]
        public A.Inner inner;
    }
}
