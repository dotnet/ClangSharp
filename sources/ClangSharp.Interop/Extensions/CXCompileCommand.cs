// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXCompileCommand : IEquatable<CXCompileCommand>
{
    public CXCompileCommand(IntPtr handle)
    {
        Handle = handle;
    }

    public readonly CXString Directory => clang.CompileCommand_getDirectory(this);

    public readonly CXString Filename => clang.CompileCommand_getFilename(this);

    public IntPtr Handle { get; set; }

    public readonly uint NumArgs => clang.CompileCommand_getNumArgs(this);

    public readonly uint NumMappedSources => clang.CompileCommand_getNumMappedSources(this);

    public static explicit operator CXCompileCommand(void* value) => new CXCompileCommand((IntPtr)value);

    public static implicit operator void*(CXCompileCommand value) => (void*)value.Handle;

    public static bool operator ==(CXCompileCommand left, CXCompileCommand right) => left.Handle == right.Handle;

    public static bool operator !=(CXCompileCommand left, CXCompileCommand right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXCompileCommand other) && Equals(other);

    public readonly bool Equals(CXCompileCommand other) => this == other;

    public readonly CXString GetArg(uint index) => clang.CompileCommand_getArg(this, index);

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly CXString GetMappedSourceContent(uint index) => clang.CompileCommand_getMappedSourceContent(this, index);

    public readonly CXString GetMappedSourcePath(uint index) => clang.CompileCommand_getMappedSourcePath(this, index);
}
