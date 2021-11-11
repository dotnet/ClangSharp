// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIndexAction : IDisposable, IEquatable<CXIndexAction>
    {
        public CXIndexAction(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIndexAction(void* value) => new CXIndexAction((IntPtr)value);

        public static implicit operator void*(CXIndexAction value) => (void*)value.Handle;

        public static bool operator ==(CXIndexAction left, CXIndexAction right) => left.Handle == right.Handle;

        public static bool operator !=(CXIndexAction left, CXIndexAction right) => left.Handle != right.Handle;

        public static CXIndexAction Create(CXIndex index) => (CXIndexAction)clang.IndexAction_create(index);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.IndexAction_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXIndexAction other) && Equals(other);

        public bool Equals(CXIndexAction other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public bool TryIndexSourceFile(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, string sourceFilename, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, out CXTranslationUnit tu, CXTranslationUnit_Flags tuOptions)
        {
            using var marshaledSourceFilename = new MarshaledString(sourceFilename);
            using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

            fixed (IndexerCallbacks* pIndexCallbacks = indexCallbacks)
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            fixed (CXTranslationUnit* pTU = &tu)
            {
                var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.indexSourceFile(this, clientData, pIndexCallbacks, (uint)indexCallbacks.Length, (uint)indexOptions, marshaledSourceFilename, pCommandLineArgs, commandLineArgs.Length, pUnsavedFiles, (uint)unsavedFiles.Length, (CXTranslationUnitImpl**)pTU, (uint)tuOptions) == 0;
            }
        }

        public bool TryIndexSourceFileFullArgv(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, string sourceFilename, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, out CXTranslationUnit tu, CXTranslationUnit_Flags tuOptions)
        {
            using var marshaledSourceFilename = new MarshaledString(sourceFilename);
            using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

            fixed (IndexerCallbacks* pIndexCallbacks = indexCallbacks)
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            fixed (CXTranslationUnit* pTU = &tu)
            {
                var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.indexSourceFileFullArgv(this, clientData, pIndexCallbacks, (uint)indexCallbacks.Length, (uint)indexOptions, marshaledSourceFilename, pCommandLineArgs, commandLineArgs.Length, pUnsavedFiles, (uint)unsavedFiles.Length, (CXTranslationUnitImpl**)pTU, (uint)tuOptions) == 0;
            }
        }

        public bool TryIndexTranslationUnit(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, CXTranslationUnit tu)
        {
            fixed (IndexerCallbacks* pIndexCallbacks = indexCallbacks)
            {
                return clang.indexTranslationUnit(this, clientData, pIndexCallbacks, (uint)indexCallbacks.Length, (uint)indexOptions, tu) == 0;
            }
        }
    }
}
