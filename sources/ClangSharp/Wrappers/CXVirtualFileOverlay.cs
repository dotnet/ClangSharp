using System;

namespace ClangSharp
{
    public unsafe partial struct CXVirtualFileOverlay
    {
        public CXVirtualFileOverlay(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static implicit operator CXVirtualFileOverlay(CXVirtualFileOverlayImpl* value)
        {
            return new CXVirtualFileOverlay((IntPtr)value);
        }

        public static implicit operator CXVirtualFileOverlayImpl*(CXVirtualFileOverlay value)
        {
            return (CXVirtualFileOverlayImpl*)value.Pointer;
        }
    }
}
