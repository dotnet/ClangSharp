// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTranslationUnit : IDisposable, IEquatable<CXTranslationUnit>
    {
        public CXTranslationUnit(IntPtr handle)
        {
            Handle = handle;
        }

        public static CXTranslationUnit_Flags DefaultEditingOptions => (CXTranslationUnit_Flags)clang.defaultEditingTranslationUnitOptions();

        public CXSourceRangeList* AllSkippedRanges => clang.getAllSkippedRanges(this);

        public CXCursor Cursor => clang.getTranslationUnitCursor(this);

        public CXReparse_Flags DefaultReparseOptions => (CXReparse_Flags)clang.defaultReparseOptions(this);

        public CXSaveTranslationUnit_Flags DefaultSaveOptions => (CXSaveTranslationUnit_Flags)clang.defaultSaveOptions(this);

        public CXDiagnosticSet DiagnosticSet => (CXDiagnosticSet)clang.getDiagnosticSetFromTU(this);

        public IntPtr Handle { get; set; }

        public uint NumDiagnostics => clang.getNumDiagnostics(this);

        public CXTUResourceUsage ResourceUsage => clang.getCXTUResourceUsage(this);

        public CXString Spelling => clang.getTranslationUnitSpelling(this);

        public CXTargetInfo TargetInfo => clang.getTranslationUnitTargetInfo(this);

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

        public void AnnotateTokens(ReadOnlySpan<CXToken> tokens, Span<CXCursor> cursors)
        {
            fixed (CXToken* pTokens = tokens)
            fixed (CXCursor* pCursors = cursors)
            {
                clang.annotateTokens(this, pTokens, (uint)tokens.Length, pCursors);
            }
        }

        public CXCodeCompleteResults* CodeCompleteAt(string completeFilename, uint completeLine, uint completeColumn, ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXCodeComplete_Flags options)
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

        public void DisposeTokens(ReadOnlySpan<CXToken> tokens)
        {
            fixed (CXToken* pTokens = tokens)
            {
                clang.disposeTokens(this, pTokens, (uint)tokens.Length);
            }
        }

        public override bool Equals(object obj) => (obj is CXTranslationUnit other) && Equals(other);

        public bool Equals(CXTranslationUnit other) => this == other;

        public CXResult FindIncludesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findIncludesInFile(this, file, visitor);

        public CXCursor GetCursor(CXSourceLocation location) => clang.getCursor(this, location);

        public CXDiagnostic GetDiagnostic(uint index) => (CXDiagnostic)clang.getDiagnostic(this, index);

        public CXFile GetFile(string fileName)
        {
            using var marshaledFileName = new MarshaledString(fileName);
            return GetFile(marshaledFileName.AsSpan());
        }

        public CXFile GetFile(ReadOnlySpan<byte> fileName)
        {
            fixed (byte* pFileName = fileName)
            {
                return (CXFile)clang.getFile(this, (sbyte*)pFileName);
            }
        }

        public ReadOnlySpan<byte> GetFileContents(CXFile file, out UIntPtr size)
        {
            fixed (UIntPtr* pSize = &size)
            {
                var pFileContents = clang.getFileContents(this, file, pSize);
                return new ReadOnlySpan<byte>(pFileContents, (int)size);
            }
        }

        public override int GetHashCode() => Handle.GetHashCode();

        public void GetInclusions(CXInclusionVisitor visitor, CXClientData clientData)
        {
            var pVisitor = Marshal.GetFunctionPointerForDelegate(visitor);
            clang.getInclusions(this, pVisitor, clientData);
            GC.KeepAlive(visitor);
        }

        public CXSourceLocation GetLocation(CXFile file, uint line, uint column) => clang.getLocation(this, file, line, column);

        public CXSourceLocation GetLocationForOffset(CXFile file, uint offset) => clang.getLocationForOffset(this, file, offset);

        public CXModule GetModuleForFile(CXFile file) => (CXModule)clang.getModuleForFile(this, file);

        public CXSourceRangeList* GetSkippedRanges(CXFile file) => clang.getSkippedRanges(this, file);

        public CXToken* GetToken(CXSourceLocation sourceLocation) => clang.getToken(this, sourceLocation);

        public bool IsFileMultipleIncludeGuarded(CXFile file) => clang.isFileMultipleIncludeGuarded(this, file) != 0;

        public CXErrorCode Reparse(ReadOnlySpan<CXUnsavedFile> unsavedFiles, CXReparse_Flags options)
        {
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            {
                return (CXErrorCode)clang.reparseTranslationUnit(this, (uint)unsavedFiles.Length, pUnsavedFiles, (uint)options);
            }
        }

        public CXSaveError Save(string fileName, CXSaveTranslationUnit_Flags options)
        {
            using var marshaledFileName = new MarshaledString(fileName);
            return (CXSaveError)clang.saveTranslationUnit(this, marshaledFileName, (uint)options);
        }

        public bool Suspend() => clang.suspendTranslationUnit(this) != 0;

        public Span<CXToken> Tokenize(CXSourceRange sourceRange)
        {
            CXToken* pTokens; uint numTokens;
            clang.tokenize(this, sourceRange, &pTokens, &numTokens);

#if NETSTANDARD
            var result = new CXToken[checked((int)numTokens)];

            fixed (CXToken* pResult = result)
            {
                var size = sizeof(CXToken) * numTokens;
                Buffer.MemoryCopy(pTokens, pResult, size, size);
            }

            return result;
#else
            return new Span<CXToken>(pTokens, (int)numTokens);
#endif
        }

        public override string ToString() => Spelling.ToString();
    }
}
