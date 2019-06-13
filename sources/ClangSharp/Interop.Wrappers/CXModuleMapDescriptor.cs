using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModuleMapDescriptor
    {
        public CXModuleMapDescriptor(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static implicit operator CXModuleMapDescriptor(CXModuleMapDescriptorImpl* value)
        {
            return new CXModuleMapDescriptor((IntPtr)value);
        }

        public static implicit operator CXModuleMapDescriptorImpl*(CXModuleMapDescriptor value)
        {
            return (CXModuleMapDescriptorImpl*)value.Pointer;
        }
    }
}
