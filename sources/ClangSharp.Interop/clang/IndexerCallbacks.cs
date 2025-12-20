// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public unsafe partial struct IndexerCallbacks
{
    [NativeTypeName("int (*)(CXClientData, void *)")]
    public delegate* unmanaged[Cdecl]<void*, void*, int> abortQuery;

    [NativeTypeName("void (*)(CXClientData, CXDiagnosticSet, void *)")]
    public delegate* unmanaged[Cdecl]<void*, void*, void*, void> diagnostic;

    [NativeTypeName("CXIdxClientFile (*)(CXClientData, CXFile, void *)")]
    public delegate* unmanaged[Cdecl]<void*, void*, void*, void*> enteredMainFile;

    [NativeTypeName("CXIdxClientFile (*)(CXClientData, const CXIdxIncludedFileInfo *)")]
    public delegate* unmanaged[Cdecl]<void*, CXIdxIncludedFileInfo*, void*> ppIncludedFile;

    [NativeTypeName("CXIdxClientASTFile (*)(CXClientData, const CXIdxImportedASTFileInfo *)")]
    public delegate* unmanaged[Cdecl]<void*, CXIdxImportedASTFileInfo*, void*> importedASTFile;

    [NativeTypeName("CXIdxClientContainer (*)(CXClientData, void *)")]
    public delegate* unmanaged[Cdecl]<void*, void*, void*> startedTranslationUnit;

    [NativeTypeName("void (*)(CXClientData, const CXIdxDeclInfo *)")]
    public delegate* unmanaged[Cdecl]<void*, CXIdxDeclInfo*, void> indexDeclaration;

    [NativeTypeName("void (*)(CXClientData, const CXIdxEntityRefInfo *)")]
    public delegate* unmanaged[Cdecl]<void*, CXIdxEntityRefInfo*, void> indexEntityReference;
}
