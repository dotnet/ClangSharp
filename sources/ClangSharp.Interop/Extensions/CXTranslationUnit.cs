// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

public unsafe partial struct CXTranslationUnit(IntPtr handle) : IDisposable, IEquatable<CXTranslationUnit>
{
    public static CXTranslationUnit_Flags DefaultEditingOptions => (CXTranslationUnit_Flags)clang.defaultEditingTranslationUnitOptions();

    public readonly CXSourceRangeList* AllSkippedRanges => clang.getAllSkippedRanges(this);

    public readonly CXCursor Cursor => clang.getTranslationUnitCursor(this);

    public readonly CXReparse_Flags DefaultReparseOptions => (CXReparse_Flags)clang.defaultReparseOptions(this);

    public readonly CXSaveTranslationUnit_Flags DefaultSaveOptions => (CXSaveTranslationUnit_Flags)clang.defaultSaveOptions(this);

    public readonly CXDiagnosticSet DiagnosticSet => (CXDiagnosticSet)clang.getDiagnosticSetFromTU(this);

    public IntPtr Handle { get; set; } = handle;

    public readonly uint NumDiagnostics => clang.getNumDiagnostics(this);

    public readonly CXTUResourceUsage ResourceUsage => clang.getCXTUResourceUsage(this);

    public readonly CXString Spelling => clang.getTranslationUnitSpelling(this);

    public readonly CXTargetInfo TargetInfo => clang.getTranslationUnitTargetInfo(this);

    public static implicit operator CXTranslationUnit(CXTranslationUnitImpl* value) => new CXTranslationUnit((IntPtr)value);

    public static implicit operator CXTranslationUnitImpl*(CXTranslationUnit value) => (CXTranslationUnitImpl*)value.Handle;

    public static bool operator ==(CXTranslationUnit left, CXTranslationUnit right) => left.Handle == right.Handle;

    public static bool operator !=(CXTranslationUnit left, CXTranslationUnit right) => left.Handle != right.Handle;

    public static CXTranslationUnit Create(CXIndex index, string astFileName)
    {
        using var marshaledAstFileName = new MarshaledString(astFileName);
        return clang.createTranslationUnit(index, marshaledAstFileName);
    }

    public static CXErrorCode Create(CXIndex index, string astFileName, out CXTranslationUnit translationUnit)
    {
        using var marshaledAstFileName = new MarshaledString(astFileName);

        fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
        {
            return clang.createTranslationUnit2(index, marshaledAstFileName, (CXTranslationUnitImpl**)pTranslationUnit);
        }
    }

    public static CXTranslationUnit CreateFromSourceFile(CXIndex index, string sourceFileName, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles)
    {
        using var marshaledSourceFileName = new MarshaledString(sourceFileName);
        using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        {
            var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
            marshaledCommandLineArgs.Fill(pCommandLineArgs);
            return clang.createTranslationUnitFromSourceFile(index, marshaledSourceFileName, commandLineArgs.Length, pCommandLineArgs, (uint)unsavedFiles.Length, pUnsavedFiles);
        }
    }

    public static CXTranslationUnit Parse(CXIndex index, string sourceFileName, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXTranslationUnit_Flags options)
    {
        using var marshaledSourceFileName = new MarshaledString(sourceFileName);
        using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        {
            var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
            marshaledCommandLineArgs.Fill(pCommandLineArgs);
            return clang.parseTranslationUnit(index, marshaledSourceFileName, pCommandLineArgs, commandLineArgs.Length, pUnsavedFiles, (uint)unsavedFiles.Length, (uint)options);
        }
    }

    public static CXErrorCode TryParse(CXIndex index, string sourceFileName, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit)
    {
        using var marshaledSourceFileName = new MarshaledString(sourceFileName);
        using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
        {
            var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
            marshaledCommandLineArgs.Fill(pCommandLineArgs);
            return clang.parseTranslationUnit2(index, marshaledSourceFileName, pCommandLineArgs, commandLineArgs.Length, pUnsavedFiles, (uint)unsavedFiles.Length, (uint)options, (CXTranslationUnitImpl**)pTranslationUnit);
        }
    }

    public static CXErrorCode TryParseFullArgv(CXIndex index, string sourceFileName, ReadOnlySpan<string> commandLineArgs, ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit)
    {
        using var marshaledSourceFileName = new MarshaledString(sourceFileName);
        using var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs);

        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
        {
            var pCommandLineArgs = stackalloc sbyte*[commandLineArgs.Length];
            marshaledCommandLineArgs.Fill(pCommandLineArgs);
            return clang.parseTranslationUnit2FullArgv(index, marshaledSourceFileName, pCommandLineArgs, commandLineArgs.Length, pUnsavedFiles, (uint)unsavedFiles.Length, (uint)options, (CXTranslationUnitImpl**)pTranslationUnit);
        }
    }

    public readonly void AnnotateTokens(ReadOnlySpan<CXToken> tokens, Span<CXCursor> cursors)
    {
        fixed (CXToken* pTokens = tokens)
        fixed (CXCursor* pCursors = cursors)
        {
            clang.annotateTokens(this, pTokens, (uint)tokens.Length, pCursors);
        }
    }

    public readonly CXCodeCompleteResults* CodeCompleteAt(string completeFilename, uint completeLine, uint completeColumn, ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXCodeComplete_Flags options)
    {
        using var marshaledCompleteFilename = new MarshaledString(completeFilename);

        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        {
            return clang.codeCompleteAt(this, marshaledCompleteFilename, completeLine, completeColumn, pUnsavedFiles, (uint)unsavedFiles.Length, (uint)options);
        }
    }

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.disposeTranslationUnit(this);
            Handle = IntPtr.Zero;
        }
    }

    public readonly void DisposeTokens(ReadOnlySpan<CXToken> tokens)
    {
        fixed (CXToken* pTokens = tokens)
        {
            clang.disposeTokens(this, pTokens, (uint)tokens.Length);
        }
    }

    public override readonly bool Equals(object? obj) => (obj is CXTranslationUnit other) && Equals(other);

    public readonly bool Equals(CXTranslationUnit other) => this == other;

    public readonly CXResult FindIncludesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findIncludesInFile(this, file, visitor);

    public readonly CXCursor GetCursor(CXSourceLocation location) => clang.getCursor(this, location);

    public readonly CXDiagnostic GetDiagnostic(uint index) => (CXDiagnostic)clang.getDiagnostic(this, index);

    public readonly CXFile GetFile(string fileName)
    {
        using var marshaledFileName = new MarshaledString(fileName);
        return GetFile(marshaledFileName.AsSpan());
    }

    public readonly CXFile GetFile(ReadOnlySpan<byte> fileName)
    {
        fixed (byte* pFileName = fileName)
        {
            return (CXFile)clang.getFile(this, (sbyte*)pFileName);
        }
    }

    public readonly ReadOnlySpan<byte> GetFileContents(CXFile file, out UIntPtr size)
    {
        fixed (UIntPtr* pSize = &size)
        {
            var pFileContents = clang.getFileContents(this, file, pSize);
            return new ReadOnlySpan<byte>(pFileContents, (int)size);
        }
    }

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly void GetInclusions(CXInclusionVisitor visitor, CXClientData clientData)
    {
        var pVisitor = (delegate* unmanaged[Cdecl]<void*, CXSourceLocation*, uint, void*, void>)Marshal.GetFunctionPointerForDelegate(visitor);
        GetInclusions(pVisitor, clientData);
        GC.KeepAlive(visitor);
    }

    public readonly void GetInclusions(delegate* unmanaged[Cdecl]<void*, CXSourceLocation*, uint, void*, void> visitor, CXClientData clientData)
    {
        clang.getInclusions(this, visitor, clientData);
    }

    public readonly CXSourceLocation GetLocation(CXFile file, uint line, uint column) => clang.getLocation(this, file, line, column);

    public readonly CXSourceLocation GetLocationForOffset(CXFile file, uint offset) => clang.getLocationForOffset(this, file, offset);

    public readonly CXModule GetModuleForFile(CXFile file) => (CXModule)clang.getModuleForFile(this, file);

    public readonly CXSourceRangeList* GetSkippedRanges(CXFile file) => clang.getSkippedRanges(this, file);

    public readonly CXToken* GetToken(CXSourceLocation sourceLocation) => clang.getToken(this, sourceLocation);

    public readonly bool IsFileMultipleIncludeGuarded(CXFile file) => clang.isFileMultipleIncludeGuarded(this, file) != 0;

    public readonly CXErrorCode Reparse(ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXReparse_Flags options)
    {
        fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
        {
            return (CXErrorCode)clang.reparseTranslationUnit(this, (uint)unsavedFiles.Length, pUnsavedFiles, (uint)options);
        }
    }

    public readonly CXSaveError Save(string fileName, CXSaveTranslationUnit_Flags options)
    {
        using var marshaledFileName = new MarshaledString(fileName);
        return (CXSaveError)clang.saveTranslationUnit(this, marshaledFileName, (uint)options);
    }

    public readonly bool Suspend() => clang.suspendTranslationUnit(this) != 0;

    public readonly Span<CXToken> Tokenize(CXSourceRange sourceRange)
    {
        CXToken* pTokens; uint numTokens;
        clang.tokenize(this, sourceRange, &pTokens, &numTokens);
        return new Span<CXToken>(pTokens, (int)numTokens);
    }

    public override readonly string ToString() => Spelling.ToString();
}
