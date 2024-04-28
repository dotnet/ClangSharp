// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXIndexAction(IntPtr handle) : IDisposable, IEquatable<CXIndexAction>
{
    public IntPtr Handle { get; set; } = handle;

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

    public override readonly bool Equals(object? obj) => (obj is CXIndexAction other) && Equals(other);

    public readonly bool Equals(CXIndexAction other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly bool TryIndexSourceFile(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, string sourceFilename, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, out CXTranslationUnit tu, CXTranslationUnit_Flags tuOptions)
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

    public readonly bool TryIndexSourceFileFullArgv(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, string sourceFilename, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, out CXTranslationUnit tu, CXTranslationUnit_Flags tuOptions)
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

    public readonly bool TryIndexTranslationUnit(CXClientData clientData, ReadOnlySpan<IndexerCallbacks> indexCallbacks, CXIndexOptFlags indexOptions, CXTranslationUnit tu)
    {
        fixed (IndexerCallbacks* pIndexCallbacks = indexCallbacks)
        {
            return clang.indexTranslationUnit(this, clientData, pIndexCallbacks, (uint)indexCallbacks.Length, (uint)indexOptions, tu) == 0;
        }
    }
}
