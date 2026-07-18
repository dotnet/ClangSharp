namespace ClangSharp.Test
{
    public partial struct b3Vec3
    {
        public float x;

        public float y;

        public float z;
    }

    public static partial class Methods
    {
        [return: NativeTypeName("struct b3Vec3")]
        public static b3Vec3 b3Negate([NativeTypeName("struct b3Vec3")] b3Vec3 a)
        {
            return new b3Vec3
            {
                x = -a.x,
                y = -a.y,
                z = -a.z,
            };
        }
    }
}
