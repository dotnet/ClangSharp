// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

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

    public override readonly bool Equals(object? obj) => (obj is CXModuleMapDescriptor other) && Equals(other);

    public readonly bool Equals(CXModuleMapDescriptor other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly CXErrorCode SetFrameworkModuleName(string name)
    {
        var marshaledName = new MarshaledString(name);
        return clang.ModuleMapDescriptor_setFrameworkModuleName(this, marshaledName);
    }

    public readonly CXErrorCode SetUmbrellaHeader(string name)
    {
        using var marshaledName = new MarshaledString(name);
        return clang.ModuleMapDescriptor_setUmbrellaHeader(this, marshaledName);
    }

    public readonly Span<byte> WriteToBuffer(uint options, out CXErrorCode errorCode)
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
