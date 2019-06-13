using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModuleMapDescriptor : IDisposable, IEquatable<CXModuleMapDescriptor>
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

        public static CXModuleMapDescriptor Create(uint options)
        {
            return clang.ModuleMapDescriptor_create(options);
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.ModuleMapDescriptor_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXModuleMapDescriptor other) && Equals(other);

        public bool Equals(CXModuleMapDescriptor other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();

        public CXErrorCode SetFrameworkModuleName(string name)
        {
            using (var marshaledName = new MarshaledString(name))
            {
                return clang.ModuleMapDescriptor_setFrameworkModuleName(this, marshaledName);
            }
        }

        public CXErrorCode SetUmbrellaHeader(string name)
        {
            using (var marshaledName = new MarshaledString(name))
            {
                return clang.ModuleMapDescriptor_setUmbrellaHeader(this, marshaledName);
            }
        }

        public Span<byte> WriteToBuffer(uint options, out CXErrorCode errorCode)
        {
            sbyte* pBuffer; uint size;
            errorCode = clang.ModuleMapDescriptor_writeToBuffer(this, options, &pBuffer, &size);
            return new Span<byte>(pBuffer, (int)size);
        }
    }
}
