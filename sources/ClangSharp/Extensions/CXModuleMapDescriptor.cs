using System;

namespace ClangSharp
{
    public unsafe partial struct CXModuleMapDescriptor : IDisposable
    {
        public static CXModuleMapDescriptor Create(uint options)
        {
            return clang.ModuleMapDescriptor_create(options);
        }

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.ModuleMapDescriptor_dispose(this);
                Pointer = IntPtr.Zero;
            }
        }

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
