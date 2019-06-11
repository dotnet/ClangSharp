using System;

namespace ClangSharp
{
    public partial struct IndexerCallbacks
    {
        [NativeTypeName("int (*)(CXClientData, void *)")]
        public IntPtr abortQuery;

        [NativeTypeName("void (*)(CXClientData, CXDiagnosticSet, void *)")]
        public IntPtr diagnostic;

        [NativeTypeName("CXIdxClientFile (*)(CXClientData, CXFile, void *)")]
        public IntPtr enteredMainFile;

        [NativeTypeName("CXIdxClientFile (*)(CXClientData, const CXIdxIncludedFileInfo *)")]
        public IntPtr ppIncludedFile;

        [NativeTypeName("CXIdxClientASTFile (*)(CXClientData, const CXIdxImportedASTFileInfo *)")]
        public IntPtr importedASTFile;

        [NativeTypeName("CXIdxClientContainer (*)(CXClientData, void *)")]
        public IntPtr startedTranslationUnit;

        [NativeTypeName("void (*)(CXClientData, const CXIdxDeclInfo *)")]
        public IntPtr indexDeclaration;

        [NativeTypeName("void (*)(CXClientData, const CXIdxEntityRefInfo *)")]
        public IntPtr indexEntityReference;
    }
}
