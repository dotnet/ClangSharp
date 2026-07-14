using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        public static int MyFunction1()
        {
            _MyFunction1_e__Union u = new _MyFunction1_e__Union();

            u.a = 0;
            return u.a;
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _MyFunction1_e__Union
        {
            [FieldOffset(0)]
            public int a;

            [FieldOffset(0)]
            public float b;
        }

        public static int MyFunction2()
        {
            _MyFunction2_e__Union u = new _MyFunction2_e__Union();

            u.a = 0;
            return u.a;
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _MyFunction2_e__Union
        {
            [FieldOffset(0)]
            public int a;

            [FieldOffset(0)]
            public float b;
        }
    }
}
