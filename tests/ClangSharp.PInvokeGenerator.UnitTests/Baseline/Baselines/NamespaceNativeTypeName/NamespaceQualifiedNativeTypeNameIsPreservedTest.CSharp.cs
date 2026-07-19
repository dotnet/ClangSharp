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

        [NativeTypeName("const Ns::Point *")]
        public Point* constPointPtr;

        [NativeTypeName("const Ns::Real *")]
        public float* constRealPtr;

        [NativeTypeName("struct Ns::Point *")]
        public Point* elabPointPtr;

        [NativeTypeName("const struct Ns::Point *")]
        public Point* constElabPointPtr;

        [NativeTypeName("enum Ns::Status *")]
        public Status* elabStatusPtr;

        [NativeTypeName("Ns::Real")]
        public float r;

        [NativeTypeName("Ns::Status")]
        public Status status;
    }
}
