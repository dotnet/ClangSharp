using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTargetInfo
    {
        public CXTargetInfo(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static implicit operator CXTargetInfo(CXTargetInfoImpl* value)
        {
            return new CXTargetInfo((IntPtr)value);
        }

        public static implicit operator CXTargetInfoImpl*(CXTargetInfo value)
        {
            return (CXTargetInfoImpl*)value.Pointer;
        }
    }
}
