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

        public CXString Spelling => clang.getTranslationUnitSpelling(this);

        public void Dispose() => clang.disposeTranslationUnit(this);

        public CXCursor GetCursor(CXSourceLocation location) => clang.getCursor(this, location);

        public CXDiagnostic GetDiagnostic(uint index) => clang.getDiagnostic(this, index);

        public CXFile GetFile(string fileName) => clang.getFile(this, fileName);

        public CXErrorCode Reparse(CXUnsavedFile[] unsavedFiles, CXReparse_Flags options) => (CXErrorCode)clang.reparseTranslationUnit(this, (uint)unsavedFiles.Length, unsavedFiles, (uint)options);

        public CXSaveError Save(string fileName, CXSaveTranslationUnit_Flags options) => (CXSaveError)clang.saveTranslationUnit(this, fileName, (uint)options);

        public bool Suspend() => clang.suspendTranslationUnit(this) != 0;

        public override string ToString() => Spelling.ToString();
    }
}
