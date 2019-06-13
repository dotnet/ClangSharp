using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModuleMapDescriptor : IEquatable<CXModuleMapDescriptor>
    {
        public CXModuleMapDescriptor(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXModuleMapDescriptor(CXModuleMapDescriptorImpl* value) => new CXModuleMapDescriptor((IntPtr)value);

        public static implicit operator CXModuleMapDescriptorImpl*(CXModuleMapDescriptor value) => (CXModuleMapDescriptorImpl*)value.Handle;

        public static bool operator ==(CXModuleMapDescriptor left, CXModuleMapDescriptor right) => left.Handle == right.Handle;

        public static bool operator !=(CXModuleMapDescriptor left, CXModuleMapDescriptor right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXModuleMapDescriptor other) && Equals(other);

        public bool Equals(CXModuleMapDescriptor other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
