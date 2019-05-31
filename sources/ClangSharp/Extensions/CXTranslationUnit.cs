using System;

namespace ClangSharp
{
    public partial struct CXTranslationUnit : IDisposable
    {
        public static CXTranslationUnit Create(CXIndex index, string astFileName) => clang.createTranslationUnit(index, astFileName);

        public static CXErrorCode Create(CXIndex index, string astFileName, out CXTranslationUnit translationUnit) => clang.createTranslationUnit2(index, astFileName, out translationUnit);

        public static CXTranslationUnit CreateFromSourceFile(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles) => clang.createTranslationUnitFromSourceFile(index, sourceFileName, commandLineArgs.Length, commandLineArgs, (uint)unsavedFiles.Length, unsavedFiles);

        public static CXTranslationUnit Parse(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options) => clang.parseTranslationUnit(index, sourceFileName, commandLineArgs, commandLineArgs.Length, unsavedFiles, (uint)unsavedFiles.Length, (uint)options);

        public static CXErrorCode Parse(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit) => clang.parseTranslationUnit2(index, sourceFileName, commandLineArgs, commandLineArgs.Length, unsavedFiles, (uint)unsavedFiles.Length, (uint)options, out translationUnit);

        public static CXErrorCode ParseFullArgv(CXIndex index, string sourceFileName, string[] commandLineArgs, CXUnsavedFile[] unsavedFiles, CXTranslationUnit_Flags options, out CXTranslationUnit translationUnit) => clang.parseTranslationUnit2FullArgv(index, sourceFileName, commandLineArgs, commandLineArgs.Length, unsavedFiles, (uint)unsavedFiles.Length, (uint)options, out translationUnit);

        public CXCursor Cursor => clang.getTranslationUnitCursor(this);

        public CXReparse_Flags DefaultReparseOptions => (CXReparse_Flags)clang.defaultReparseOptions(this);

        public CXSaveTranslationUnit_Flags DefaultSaveOptions => (CXSaveTranslationUnit_Flags)clang.defaultSaveOptions(this);

        public CXDiagnosticSet DiagnosticSet => clang.getDiagnosticSetFromTU(this);

        public uint NumDiagnostics => clang.getNumDiagnostics(this);

        public CXTUResourceUsage ResourceUsage => clang.getCXTUResourceUsage(this);

        public CXString Spelling => clang.getTranslationUnitSpelling(this);

        public CXTargetInfo TargetInfo => clang.getTranslationUnitTargetInfo(this);

        public void AnnotateTokens(CXToken[] tokens, CXCursor[] cursors) => clang.annotateTokens(this, tokens, (uint)tokens.Length, cursors);

        public void Dispose() => clang.disposeTranslationUnit(this);

        public void DisposeTokens(CXToken[] tokens) => clang.disposeTokens(this, tokens, (uint)tokens.Length);

        public CXResult FindIncludesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findIncludesInFile(this, file, visitor);

        public unsafe ref CXSourceRangeList GetAllSkippedRanges() => ref *(CXSourceRangeList*)clang.getAllSkippedRanges(this);

        public CXCursor GetCursor(CXSourceLocation location) => clang.getCursor(this, location);

        public CXDiagnostic GetDiagnostic(uint index) => clang.getDiagnostic(this, index);

        public CXFile GetFile(string fileName) => clang.getFile(this, fileName);

        public string GetFileContents(CXFile file, out IntPtr size) => clang.getFileContents(this, file, out size);

        public void GetInclusions(CXInclusionVisitor visitor, CXClientData clientData) => clang.getInclusions(this, visitor, clientData);

        public CXSourceLocation GetLocation(CXFile file, uint line, uint column) => clang.getLocation(this, file, line, column);

        public CXSourceLocation GetLocationForOffset(CXFile file, uint offset) => clang.getLocationForOffset(this, file, offset);

        public CXModule GetModuleForFile(CXFile file) => clang.getModuleForFile(this, file);

        public unsafe ref CXSourceRangeList GetSkippedRanges(CXFile file) => ref *(CXSourceRangeList*)clang.getSkippedRanges(this, file);

        public unsafe ref CXToken GetToken(CXSourceLocation sourceLocation) => ref *(CXToken*)clang.getToken(this, sourceLocation);

        public bool IsFileMultipleIncludeGuarded(CXFile file) => clang.isFileMultipleIncludeGuarded(this, file) != 0;

        public CXErrorCode Reparse(CXUnsavedFile[] unsavedFiles, CXReparse_Flags options) => (CXErrorCode)clang.reparseTranslationUnit(this, (uint)unsavedFiles.Length, unsavedFiles, (uint)options);

        public CXSaveError Save(string fileName, CXSaveTranslationUnit_Flags options) => (CXSaveError)clang.saveTranslationUnit(this, fileName, (uint)options);

        public bool Suspend() => clang.suspendTranslationUnit(this) != 0;

        public void Tokenize(CXSourceRange sourceRange, out CXToken[] tokens) => clang.tokenize(this, sourceRange, out tokens, out _);

        public override string ToString() => Spelling.ToString();

    }
}
