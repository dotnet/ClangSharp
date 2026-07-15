using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct Base
    {
        public int value;
    }

    [NativeTypeName("struct Derived : Base<T>")]
    public partial struct Derived
    {
        public Base Base;
    }
}
