// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-11.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXTRANSLATIONUNIT_H
#define LIBCLANGSHARP_CXTRANSLATIONUNIT_H

#include <clang/Frontend/ASTUnit.h>
#include <clang-c/Index.h>

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
