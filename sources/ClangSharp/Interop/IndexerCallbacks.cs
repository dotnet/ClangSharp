// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-9.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
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
