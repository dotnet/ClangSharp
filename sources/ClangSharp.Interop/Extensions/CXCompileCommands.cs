// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop;

public unsafe partial struct CXCompileCommands : IDisposable, IEquatable<CXCompileCommands>, IReadOnlyCollection<CXCompileCommand>
{
    public CXCompileCommands(IntPtr handle)
    {
        Handle = handle;
    }

    public CXCompileCommand this[uint index] => GetCommand(index);

    public int Count => (int)Size;

    public IntPtr Handle { get; set; }

    public uint Size => clang.CompileCommands_getSize(this);

    public static explicit operator CXCompileCommands(void* value) => new CXCompileCommands((IntPtr)value);

    public static implicit operator void*(CXCompileCommands value) => (void*)value.Handle;

    public static bool operator ==(CXCompileCommands left, CXCompileCommands right) => left.Handle == right.Handle;

    public static bool operator !=(CXCompileCommands left, CXCompileCommands right) => left.Handle != right.Handle;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.CompilationDatabase_dispose(this);
            Handle = IntPtr.Zero;
        }
    }

    public override bool Equals(object obj) => (obj is CXCompileCommands other) && Equals(other);

    public bool Equals(CXCompileCommands other) => this == other;

    public CXCompileCommand GetCommand(uint index) => (CXCompileCommand)clang.CompileCommands_getCommand(this, index);

    public IEnumerator<CXCompileCommand> GetEnumerator()
    {
        var count = Size;

        for (var index = 0u; index < count; index++)
        {
            yield return GetCommand(index);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() => Handle.GetHashCode();
}
