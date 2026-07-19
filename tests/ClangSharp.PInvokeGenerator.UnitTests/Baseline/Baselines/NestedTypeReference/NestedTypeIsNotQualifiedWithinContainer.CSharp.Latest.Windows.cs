namespace ClangSharp.Test
{
    public unsafe partial struct Outer
    {
        public Inner field;

        public Inner* fieldPtr;

        public partial struct Inner
        {
            public int value;
        }
    }
}
