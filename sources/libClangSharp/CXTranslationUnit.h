// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXTRANSLATIONUNIT_H
#define LIBCLANGSHARP_CXTRANSLATIONUNIT_H

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang/Frontend/ASTUnit.h>
#include <clang-c/Index.h>

#pragma warning(pop)

namespace clang {
    class CIndexer;
}

namespace clang::index {
    class CommentToXMLConverter;
}

namespace clang::cxstring {
    class CXStringPool;
}

struct CXTranslationUnitImpl {
    clang::CIndexer* CIdx;
    clang::ASTUnit* TheASTUnit;
    clang::cxstring::CXStringPool* StringPool;
    void* Diagnostics;
    void* OverridenCursorsPool;
    clang::index::CommentToXMLConverter* CommentToXML;
    unsigned ParsingOptions;
    std::vector<std::string> Arguments;
};

namespace clang::cxtu {
    ASTUnit* getASTUnit(CXTranslationUnit TU);
}

#endif
