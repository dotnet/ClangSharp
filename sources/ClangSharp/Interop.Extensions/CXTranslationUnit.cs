using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTranslationUnit : IDisposable
    {
        public static CXTranslationUnit Create(CXIndex index, string astFileName)
        {
            using (var marshaledAstFileName = new MarshaledString(astFileName))
            {
                return clang.createTranslationUnit(index, marshaledAstFileName);
            }
        }

        public static CXErrorCode Create(CXIndex index, string astFileName, out CXTranslationUnit translationUnit)
        {
            using (var marshaledAstFileName = new MarshaledString(astFileName))
            fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
            {
                return clang.createTranslationUnit2(index, marshaledAstFileName, (CXTranslationUnitImpl**)pTranslationUnit);
            }
        }

        public static CXTranslationUnit CreateFromSourceFile(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles)
        {
            using (var marshaledSourceFileName = new MarshaledString(sourceFileName))
            using (var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs))
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            {
                var pCommandLineArgs = stackalloc sbyte*[marshaledCommandLineArgs.Count];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.createTranslationUnitFromSourceFile(index, marshaledSourceFileName, marshaledCommandLineArgs.Count, pCommandLineArgs, (uint)unsavedFiles?.Length, pUnsavedFiles);
            }
        }

        public static CXTranslationUnit Parse(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options)
        {
            using (var marshaledSourceFileName = new MarshaledString(sourceFileName))
            using (var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs))
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            {
                var pCommandLineArgs = stackalloc sbyte*[marshaledCommandLineArgs.Count];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.parseTranslationUnit(index, marshaledSourceFileName, pCommandLineArgs, marshaledCommandLineArgs.Count, pUnsavedFiles, (uint)unsavedFiles?.Length, (uint)options);
            }
        }

        public static CXErrorCode Parse(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit)
        {
            using (var marshaledSourceFileName = new MarshaledString(sourceFileName))
            using (var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs))
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
            {
                var pCommandLineArgs = stackalloc sbyte*[marshaledCommandLineArgs.Count];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.parseTranslationUnit2(index, marshaledSourceFileName, pCommandLineArgs, marshaledCommandLineArgs.Count, pUnsavedFiles, (uint)unsavedFiles?.Length, (uint)options, (CXTranslationUnitImpl**)pTranslationUnit);
            }
        }

        public static CXErrorCode ParseFullArgv(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit)
        {
            using (var marshaledSourceFileName = new MarshaledString(sourceFileName))
            using (var marshaledCommandLineArgs = new MarshaledStringArray(commandLineArgs))
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            fixed (CXTranslationUnit* pTranslationUnit = &translationUnit)
            {
                var pCommandLineArgs = stackalloc sbyte*[marshaledCommandLineArgs.Count];
                marshaledCommandLineArgs.Fill(pCommandLineArgs);
                return clang.parseTranslationUnit2FullArgv(index, marshaledSourceFileName, pCommandLineArgs, marshaledCommandLineArgs.Count, pUnsavedFiles, (uint)unsavedFiles?.Length, (uint)options, (CXTranslationUnitImpl**)pTranslationUnit);
            }
        }

        public CXCursor Cursor => clang.getTranslationUnitCursor(this);

        public CXReparse_Flags DefaultReparseOptions => (CXReparse_Flags)clang.defaultReparseOptions(this);

        public CXSaveTranslationUnit_Flags DefaultSaveOptions => (CXSaveTranslationUnit_Flags)clang.defaultSaveOptions(this);

        public CXDiagnosticSet DiagnosticSet => (CXDiagnosticSet)clang.getDiagnosticSetFromTU(this);

        public uint NumDiagnostics => clang.getNumDiagnostics(this);

        public CXTUResourceUsage ResourceUsage => clang.getCXTUResourceUsage(this);

        public CXString Spelling => clang.getTranslationUnitSpelling(this);

        public CXTargetInfo TargetInfo => clang.getTranslationUnitTargetInfo(this);

        public void AnnotateTokens(CXToken[] tokens, CXCursor[] cursors)
        {
            fixed (CXToken* pTokens = tokens)
            fixed (CXCursor* pCursors = cursors)
            {
                clang.annotateTokens(this, pTokens, (uint)tokens?.Length, pCursors);
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

        public void DisposeTokens(CXToken[] tokens)
        {
            fixed (CXToken* pTokens = tokens)
            {
                clang.disposeTokens(this, pTokens, (uint)tokens?.Length);
            }
        }

        public CXResult FindIncludesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findIncludesInFile(this, file, visitor);

        public unsafe ref CXSourceRangeList GetAllSkippedRanges() => ref *clang.getAllSkippedRanges(this);

        public CXCursor GetCursor(CXSourceLocation location) => clang.getCursor(this, location);

        public CXDiagnostic GetDiagnostic(uint index) => (CXDiagnostic)clang.getDiagnostic(this, index);

        public CXFile GetFile(string fileName)
        {
            using (var marshaledFileName = new MarshaledString(fileName))
            {
                return (CXFile)clang.getFile(this, marshaledFileName);
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

        public void GetInclusions(CXInclusionVisitor visitor, CXClientData clientData)
        {
            var pVisitor = Marshal.GetFunctionPointerForDelegate(visitor);
            clang.getInclusions(this, pVisitor, clientData);
            GC.KeepAlive(visitor);
        }

        public CXSourceLocation GetLocation(CXFile file, uint line, uint column) => clang.getLocation(this, file, line, column);

        public CXSourceLocation GetLocationForOffset(CXFile file, uint offset) => clang.getLocationForOffset(this, file, offset);

        public CXModule GetModuleForFile(CXFile file) => (CXModule)clang.getModuleForFile(this, file);

        public unsafe ref CXSourceRangeList GetSkippedRanges(CXFile file) => ref *clang.getSkippedRanges(this, file);

        public unsafe ref CXToken GetToken(CXSourceLocation sourceLocation) => ref *clang.getToken(this, sourceLocation);

        public bool IsFileMultipleIncludeGuarded(CXFile file) => clang.isFileMultipleIncludeGuarded(this, file) != 0;

        public CXErrorCode Reparse(CXUnsavedFile[] unsavedFiles, CXReparse_Flags options)
        {
            fixed (CXUnsavedFile* pUnsavedFiles = unsavedFiles)
            {
                return (CXErrorCode)clang.reparseTranslationUnit(this, (uint)unsavedFiles?.Length, pUnsavedFiles, (uint)options);
            }
        }

        public CXSaveError Save(string fileName, CXSaveTranslationUnit_Flags options)
        {
            using (var marshaledFileName = new MarshaledString(fileName))
            {
                return (CXSaveError)clang.saveTranslationUnit(this, marshaledFileName, (uint)options);
            }
        }

        public bool Suspend() => clang.suspendTranslationUnit(this) != 0;

        public Span<CXToken> Tokenize(CXSourceRange sourceRange)
        {
            CXToken* pTokens; uint numTokens;
            clang.tokenize(this, sourceRange, &pTokens, &numTokens);
            return new Span<CXToken>(pTokens, (int)numTokens);
        }

        public override string ToString() => Spelling.ToString();

    }
}
