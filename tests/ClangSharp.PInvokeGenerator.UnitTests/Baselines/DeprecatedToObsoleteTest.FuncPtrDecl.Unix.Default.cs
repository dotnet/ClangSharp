using System;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct0
    {
        [NativeTypeName("Callback0")]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }

    public unsafe partial struct MyStruct1
    {
        [NativeTypeName("Callback1")]
        [Obsolete]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }

    public unsafe partial struct MyStruct2
    {
        [NativeTypeName("Callback2")]
        [Obsolete("This is obsolete.")]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }

    public unsafe partial struct MyStruct3
    {
        [NativeTypeName("Callback3")]
        public delegate* unmanaged[Cdecl]<void> _callback;
    }
}
