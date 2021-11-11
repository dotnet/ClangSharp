// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompileCommand : IEquatable<CXCompileCommand>
    {
        public CXCompileCommand(IntPtr handle)
        {
            Handle = handle;
        }

        public CXString Directory => clang.CompileCommand_getDirectory(this);

        public CXString Filename => clang.CompileCommand_getFilename(this);

        public IntPtr Handle { get; set; }

        public uint NumArgs => clang.CompileCommand_getNumArgs(this);

        public uint NumMappedSources => clang.CompileCommand_getNumMappedSources(this);

        public static explicit operator CXCompileCommand(void* value) => new CXCompileCommand((IntPtr)value);

        public static implicit operator void*(CXCompileCommand value) => (void*)value.Handle;

        public static bool operator ==(CXCompileCommand left, CXCompileCommand right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompileCommand left, CXCompileCommand right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCompileCommand other) && Equals(other);

        public bool Equals(CXCompileCommand other) => this == other;

        public CXString GetArg(uint index) => clang.CompileCommand_getArg(this, index);

        public override int GetHashCode() => Handle.GetHashCode();

        public CXString GetMappedSourceContent(uint index) => clang.CompileCommand_getMappedSourceContent(this, index);

        public CXString GetMappedSourcePath(uint index) => clang.CompileCommand_getMappedSourcePath(this, index);
    }
}
