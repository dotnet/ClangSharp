// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "CXString.h"
#include <llvm/Support/MemAlloc.h>

namespace clang::cxstring {
    CXString createDup(llvm::StringRef String) {
        CXString Result;
        char* Spelling = static_cast<char*>(llvm::safe_malloc(String.size() + 1));
        memmove(Spelling, String.data(), String.size());
        Spelling[String.size()] = 0;
        Result.data = Spelling;
        Result.private_flags = (unsigned)CXS_Malloc;
        return Result;
    }

    CXString createEmpty() {
        CXString Str;
        Str.data = "";
        Str.private_flags = CXS_Unmanaged;
        return Str;
    }

    CXString createRef(const char* String) {
        if (String && String[0] == '\0')
            return createEmpty();

        CXString Str;
        Str.data = String;
        Str.private_flags = CXS_Unmanaged;
        return Str;
    }
}
