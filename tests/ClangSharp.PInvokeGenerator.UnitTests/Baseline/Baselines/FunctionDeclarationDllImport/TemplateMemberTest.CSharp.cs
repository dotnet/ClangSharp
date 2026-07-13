using System;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("MyTemplate<float *>")]
        public MyTemplate<IntPtr> a;
    }
}
