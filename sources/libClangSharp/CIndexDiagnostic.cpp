// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-12.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "CIndexDiagnostic.h"

namespace clang {
    CXDiagnosticSetImpl::~CXDiagnosticSetImpl() { }

    void CXDiagnosticSetImpl::appendDiagnostic(std::unique_ptr<CXDiagnosticImpl> D) {
        Diagnostics.push_back(std::move(D));
    }

    CXDiagnosticImpl::~CXDiagnosticImpl() { }
}
