using System;
using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct TestInterface_
    {
        [NativeTypeName("int (*)(MyStruct *)")]
        public IntPtr TestMethod;
    }

    public unsafe partial struct MyStruct_
    {
        [NativeTypeName("const struct TestInterface_ *")]
        public TestInterface_* functions;

        public int TestMethod()
        {
            return functions->TestMethod((MyStruct_*)Unsafe.AsPointer(ref this));
        }
    }
}
