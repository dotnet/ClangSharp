namespace ClangSharp.Test
{
    public partial struct Point
    {
        [NativeTypeName("Ns::Real")]
        public float X;

        [NativeTypeName("Ns::Real")]
        public float Y;
    }

    public enum Status
    {
        Err = -1,
        Ok = 0,
    }

    public unsafe partial struct Holder
    {
        [NativeTypeName("Ns::Point")]
        public Point point;

        [NativeTypeName("Ns::Point *")]
        public Point* pointPtr;

        [NativeTypeName("Ns::Real")]
        public float r;

        [NativeTypeName("Ns::Status")]
        public Status status;
    }
}
