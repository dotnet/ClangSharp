// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-11.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXSTRING_H
#define LIBCLANGSHARP_CXSTRING_H

#include <clang-c/CXString.h>
#include <llvm/ADT/StringRef.h>

/// Describes the kind of underlying data in CXString.
enum CXStringFlag {
    /// CXString contains a 'const char*' that it doesn't own.
    CXS_Unmanaged,

    /// CXString contains a 'const char*' that it allocated with malloc().
    CXS_Malloc,

    /// CXString contains a CXStringBuf that needs to be returned to the
    /// CXStringPool.
    CXS_StringBuf
};

namespace clang::cxstring {
    /// Create a CXString object from a StringRef.  New CXString will
    /// contain a copy of \p String.
    ///
    /// \p String can be changed or freed by the caller.
    CXString createDup(llvm::StringRef String);

    /// Create a CXString object for an empty "" string.
    CXString createEmpty();

    /// Create a CXString object from a nul-terminated C string.  New
    /// CXString may contain a pointer to \p String.
    ///
    /// \p String should not be changed by the caller afterwards.
    CXString createRef(const char* String);
}

#endif
