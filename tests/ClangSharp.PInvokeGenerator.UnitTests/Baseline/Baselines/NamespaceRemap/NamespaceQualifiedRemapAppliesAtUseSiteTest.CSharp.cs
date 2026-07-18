namespace ClangSharp.Test
{
    public partial struct NsPoint
    {
        public int X;

        public int Y;
    }

    public enum NsStatus
    {
        Err = -1,
        Ok = 0,
    }

    public unsafe partial struct Holder
    {
        [NativeTypeName("Ns::Point")]
        public NsPoint point;

        [NativeTypeName("Ns::Point *")]
        public NsPoint* pointPtr;

        [NativeTypeName("Ns::Status")]
        public NsStatus status;
    }
}
