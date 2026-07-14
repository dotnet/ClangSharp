namespace ClangSharp.Test
{
    public partial struct Vector3
    {
        public float x;

        public float y;

        public float z;
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

            cardinalAxis = tmp;
            return cardinalAxis;
        }
    }
}
