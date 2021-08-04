// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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

        public static CXModuleMapDescriptor Create(uint options) => clang.ModuleMapDescriptor_create(options);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.ModuleMapDescriptor_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXModuleMapDescriptor other) && Equals(other);

        public bool Equals(CXModuleMapDescriptor other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public CXErrorCode SetFrameworkModuleName(string name)
        {
            var marshaledName = new MarshaledString(name);
            return clang.ModuleMapDescriptor_setFrameworkModuleName(this, marshaledName);
        }

        public CXErrorCode SetUmbrellaHeader(string name)
        {
            using var marshaledName = new MarshaledString(name);
            return clang.ModuleMapDescriptor_setUmbrellaHeader(this, marshaledName);
        }

        public Span<byte> WriteToBuffer(uint options, out CXErrorCode errorCode)
        {
            sbyte* pBuffer; uint size;
            errorCode = clang.ModuleMapDescriptor_writeToBuffer(this, options, &pBuffer, &size);

#if NETSTANDARD
            var result = new byte[checked((int)size)];

            fixed (byte* pResult = result)
            {
                Buffer.MemoryCopy(pBuffer, pResult, size, size);
            }

            return result;
#else
            return new Span<byte>(pBuffer, (int)size);
#endif
        }
    }
}
