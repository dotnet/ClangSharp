using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Vector3
    {
        public float x;

        public float y;

        public float z;

        [return: NativeTypeName("Vector3 &")]
        public Vector3* op_Assign([NativeTypeName("const Vector3 &")] Vector3* other)
        {
            x = other->x;
            y = other->y;
            z = other->z;
            return (Vector3*)Unsafe.AsPointer(ref this);
        }
    }

    public static partial class Methods
    {
        public static Vector3 MyFunction(Vector3 v)
        {
            Vector3 cardinalAxis = new Vector3
            {
                x = 1.0f,
                y = 0.0f,
                z = 0.0f,
            };
            Vector3 tmp = new Vector3
            {
                x = 0.0f,
                y = 1.0f,
                z = 0.0f,
            };

            cardinalAxis.op_Assign(&tmp);
            return cardinalAxis;
        }
    }
}
